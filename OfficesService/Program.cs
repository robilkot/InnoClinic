using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficesService.Data.Models;
using OfficesService.Models.MapperProfiles;
using OfficesService.Repository;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

var authorityString = Environment.GetEnvironmentVariable("IdentityPath") ?? builder.Configuration["IdentityPath"];

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

builder.Services.AddAutoMapper(typeof(OfficesControllerProfile));
// todo: scoped vs transient?
builder.Services.AddTransient<IRepository<DbOfficeModel>, OfficeRepository>();

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

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
