using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace IdentityServer.Core.Data
{
    public static class DatabaseInitializer
    {
        public static void PopulateIdentityServer(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            foreach (var client in Config.Clients)
            {
                var item = context.Clients.SingleOrDefault(c => c.ClientId == client.ClientId);

                if (item != null)
                {
                    context.Remove(item);
                    context.SaveChanges();
                }
                context.Clients.Add(client.ToEntity());
                
                context.SaveChanges();
            }

            foreach (var resource in Config.ApiResources)
            {
                var item = context.ApiResources.SingleOrDefault(c => c.Name == resource.Name);

                if (item != null)
                {
                    context.Remove(item);
                    context.SaveChanges();
                }
                context.ApiResources.Add(resource.ToEntity());
                
                context.SaveChanges();
            }

            foreach (var scope in Config.ApiScopes)
            {
                var item = context.ApiScopes.SingleOrDefault(c => c.Name == scope.Name);

                if (item != null)
                {
                    context.Remove(item);
                    context.SaveChanges();
                }

                context.ApiScopes.Add(scope.ToEntity());

                context.SaveChanges();
            }
        }
    }
}
