using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var identityString = Environment.GetEnvironmentVariable("IdentityPath") ?? builder.Configuration["IdentityPath"];

builder.Services.AddMvcCore()
        .AddApiExplorer();

builder.Configuration.AddJsonFile("Configuration/ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
var clientHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = identityString;
    options.RequireHttpsMetadata = false;
    options.Audience = "gateway";
    options.BackchannelHttpHandler = clientHandler;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GatewayScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "gateway");
    });
});

builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowCors");

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

await app.UseOcelot();

ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

app.UseHttpsRedirection();

app.Run();