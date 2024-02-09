using CommonData.Constants;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using ServicesService.Consumers;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Interfaces;
using ServicesService.Infrastructure.Services;
using ServicesService.Middleware;
using ServicesService.Presentation.Models;
using ServicesService.Presentation.Models.Mappers;
using ServicesService.Presentation.Models.Validators;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

var authorityString = Environment.GetEnvironmentVariable("IdentityPath") ?? builder.Configuration["IdentityPath"];
var authorityStringOuter = Environment.GetEnvironmentVariable("IdentityPathOuter") ?? builder.Configuration["IdentityPathOuter"];
var connectionString = Environment.GetEnvironmentVariable("DbConnection") ?? builder.Configuration.GetConnectionString("DbConnection");

var rmHost = Environment.GetEnvironmentVariable("RabbitMq:Host") ?? builder.Configuration["RabbitMq:Host"];
var rmUsername = Environment.GetEnvironmentVariable("RabbitMq:Username") ?? builder.Configuration.GetValue("RabbitMq:Username", "rmuser");
var rmPassword = Environment.GetEnvironmentVariable("RabbitMq:Password") ?? builder.Configuration.GetValue("RabbitMq:Password", "rmpassword");


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", builder =>
    {
        builder.AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader();
    });
});

builder.Services.AddSingleton(new MongoClient(connectionString).GetDatabase("InnoClinicServices"));

builder.Services.AddAutoMapper(typeof(ControllerProfile));

builder.Services.AddScoped<IServiceDBService, DbService>();
builder.Services.AddScoped<ISpecializationDBService, DbService>();
builder.Services.AddScoped<ICategoryDBService, DbService>();
builder.Services.AddScoped<IValidator<ClientServiceModel>, ServiceModelValidator>();
builder.Services.AddScoped<IValidator<ClientSpecializationModel>, SpecializationModelValidator>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<SpecializationRequestConsumer>();
    x.AddConsumersFromNamespaceContaining<ServiceRequestConsumer>();

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
        Title = "Innoclinic services service API",
        Description = "An ASP.NET Core Web API for managing services",
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
                Scopes = new Dictionary<string, string> {
                    { "services", "Access services, categories and specializations" },
                }
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
    options.AddPolicy("receptionists", policy =>
        policy.RequireRole(Roles.Receptionist));
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
        setup.OAuthClientId("servicesService");
        setup.OAuthAppName("Services Service");
        //setup.OAuthScopeSeparator(" ");
        setup.OAuthUsePkce();
    });
}

app.UseInnoClinicExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
