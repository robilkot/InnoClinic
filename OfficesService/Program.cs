using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficesService.Consumers;
using OfficesService.Data.Models;
using OfficesService.Models.MapperProfiles;
using OfficesService.Repository;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

var authorityString = Environment.GetEnvironmentVariable("IdentityPath") ?? builder.Configuration["IdentityPath"];
var authorityStringOuter = Environment.GetEnvironmentVariable("IdentityPathOuter") ?? builder.Configuration["IdentityPathOuter"];

var rmHost = Environment.GetEnvironmentVariable("RabbitMq:Host") ?? builder.Configuration["RabbitMq:Host"];
var rmUsername = Environment.GetEnvironmentVariable("RabbitMq:Username") ?? builder.Configuration.GetValue("RabbitMq:Username", "rmuser");
var rmPassword = Environment.GetEnvironmentVariable("RabbitMq:Password") ?? builder.Configuration.GetValue("RabbitMq:Password", "rmpassword");


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", builder =>
    {
        builder.AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader();
    });
});

builder.Services.AddAutoMapper(typeof(OfficesControllerProfile));
builder.Services.AddScoped<IRepository<DbOfficeModel>, OfficeRepository>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<OfficeRequestConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(rmHost, "/", host =>
        {
            host.Username(rmUsername);
            host.Password(rmPassword);
        });
        cfg.ConfigureEndpoints(context);
    });
});


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Innoclinic offices service API",
        Description = "An ASP.NET Core Web API for managing offices",
    });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{authorityStringOuter}/connect/authorize"),
                TokenUrl = new Uri($"{authorityStringOuter}/connect/token"),
                Scopes = new Dictionary<string, string> { { "offices.edit", "Edit offices" } }
            }
        },
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = authorityString;
    options.RequireHttpsMetadata = false;
    options.BackchannelHttpHandler = new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicy) => true
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = true,
        ValidIssuer = authorityStringOuter,

        ValidateIssuerSigningKey = false,
        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("offices.edit", policy =>
        policy.RequireClaim("scope", "offices.edit"));
});

builder.Services.AddControllers();

var app = builder.Build();

if (bool.Parse(app.Configuration["populatedb"]!))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<IRepository<DbOfficeModel>>();
    context.Init();
}

app.UseCors("AllowCors");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(setup =>
    {
        setup.SwaggerEndpoint($"/swagger/v1/swagger.json", "Version 1.0");
        setup.OAuthClientId("officesService");
        setup.OAuthAppName("Offices Service");
        //setup.OAuthScopeSeparator(" ");
        setup.OAuthUsePkce();
    });
}

//app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
