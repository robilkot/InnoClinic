using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficesService.Data;
using OfficesService.Models.MapperProfiles;
using OfficesService.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var authorityString = Environment.GetEnvironmentVariable("IdentityPath") ?? builder.Configuration["IdentityPath"];
var connectionString = Environment.GetEnvironmentVariable("DbConnection") ?? builder.Configuration.GetConnectionString("DbConnection");

// Add services to the container.

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

builder.Services.AddDbContext<OfficesDbContext>(
    b => b.UseSqlServer(connectionString)
    .UseLazyLoadingProxies());


builder.Services.AddAutoMapper(typeof(OfficesControllerProfile));
builder.Services.AddScoped<DbService>();

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
                AuthorizationUrl = new Uri($"{authorityString}/connect/authorize"),
                TokenUrl = new Uri($"{authorityString}/connect/token"),
                Scopes = new Dictionary<string, string> { { "offices.edit", "Edit offices" } }
            }
        },
    });
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
        ValidIssuer = authorityString,
      
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
