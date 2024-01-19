// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowCors", builder =>
                {
                    builder.AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyHeader();
                });
            });

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = false;
            })
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ResolveDbContextOptions = (provider, builder) =>
                    {
                        builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication();
            //.AddGoogle(options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //    // register your IdentityServer with Google at https://console.developers.google.com
            //    // enable the Google+ API
            //    // set the redirect URI to https://localhost:5001/signin-google
            //    options.ClientId = "copy client ID from Google here";
            //    options.ClientSecret = "copy client secret from Google here";
            //});

        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                foreach (var client in Config.Clients)
                {
                    if (context.Clients.FirstOrDefault(s => s.ClientId == client.ClientId) == null)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                }
                context.SaveChanges();

                foreach (var resource in Config.IdentityResources)
                {
                    if (context.IdentityResources.FirstOrDefault(s => s.Name == resource.Name) == null)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                }
                context.SaveChanges();

                foreach (var resource in Config.ApiScopes)
                {
                    if (context.ApiScopes.FirstOrDefault(s => s.Name == resource.Name) == null)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                }
                context.SaveChanges();
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            InitializeDatabase(app);

            app.UseCors("AllowCors");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}