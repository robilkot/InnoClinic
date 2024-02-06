using CommonData.Constants;
using CommonData.Messages;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficesService.Consumers;
using ProfilesService.Consumers;
using ProfilesService.Data;
using ProfilesService.Models.MapperProfiles;
using ProfilesService.Services;
using Serilog;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", builder =>
    {
        builder.AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ProfilesDbContext>(
    b => b.UseSqlServer(connectionString));


builder.Services.AddAutoMapper(typeof(ProfilesProfile));

builder.Services.AddScoped<DbService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<OfficeUpdateConsumer>();
    x.AddConsumersFromNamespaceContaining<OfficeDeleteConsumer>();
    x.AddConsumersFromNamespaceContaining<PatientRequestConsumer>();
    x.AddConsumersFromNamespaceContaining<DoctorRequestConsumer>();

    x.AddRequestClient<OfficeRequest>(TimeSpan.FromSeconds(5));
    x.AddRequestClient<SpecializationRequest>(TimeSpan.FromSeconds(5));

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
        Title = "Innoclinic profiles service API",
        Description = "An ASP.NET Core Web API for managing profiles",
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
                    { "profiles", "Access profiles" }
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
    options.AddPolicy("receptionists", policy => 
        policy.RequireRole(Roles.Receptionist))
);

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowCors");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(setup =>
    {
        setup.SwaggerEndpoint($"/swagger/v1/swagger.json", "Version 1.0");
        setup.OAuthClientId("profilesService");
        setup.OAuthAppName("Profiles Service");
        //setup.OAuthScopeSeparator(" ");
        setup.OAuthUsePkce();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
