using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityServer.Core.Data
{
    public static class Config
    {
        public static List<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = "gatewayApi",
                ClientSecrets = { new Secret("gatewayApi".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                // todo: this is not great
                RequirePkce = false,

                RedirectUris = {
                    "https://localhost:44300/signin-oidc",
                    "http://localhost:5012/",
                    "http://localhost:5012/swagger/oauth2-redirect.html",
                    "http://127.0.0.1:5012/swagger/oauth2-redirect.html"
                },

                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "gateway", "offices", "profiles", "appointments", "documents", "services" }
            },

            new Client
            {
                ClientId = "officesService",
                ClientSecrets = { new Secret("officesService".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = {
                    "https://localhost:44300/signin-oidc",
                    "http://localhost:5001/offices",
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                    "http://127.0.0.1:5001/swagger/oauth2-redirect.html"
                },

                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "offices" }
            },

            new Client
            {
                ClientId = "profilesService",
                ClientSecrets = { new Secret("profilesService".Sha256()) },

                AllowedGrantTypes = { GrantType.AuthorizationCode },

                RedirectUris = {
                    "https://localhost:44300/signin-oidc",
                    "http://localhost:5002/patients",
                    "http://localhost:5002/doctors",
                    "http://localhost:5002/receptionists",
                    "http://localhost:5002/swagger/oauth2-redirect.html",
                    "http://127.0.0.1:5002/swagger/oauth2-redirect.html"
                },

                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "profiles" }
            },

            new Client
            {
                ClientId = "servicesService",
                ClientSecrets = { new Secret("servicesService".Sha256()) },

                AllowedGrantTypes = { GrantType.AuthorizationCode },

                RedirectUris = {
                    "https://localhost:44300/signin-oidc",
                    "http://localhost:5003/services",
                    "http://localhost:5003/swagger/oauth2-redirect.html",
                    "http://127.0.0.1:5003/swagger/oauth2-redirect.html"
                },

                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "services" }
            },

            new Client
            {
                ClientId = "appointmentsService",
                ClientSecrets = { new Secret("appointmentsService".Sha256()) },

                AllowedGrantTypes = { GrantType.AuthorizationCode },

                RedirectUris = {
                    "https://localhost:44300/signin-oidc",
                    "http://localhost:5004/appointments",
                    "http://localhost:5004/swagger/oauth2-redirect.html",
                    "http://127.0.0.1:5004/swagger/oauth2-redirect.html"
                },

                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "appointments" }
            },

            new Client
            {
                ClientId = "documentsService",
                ClientSecrets = { new Secret("documentsService".Sha256()) },

                AllowedGrantTypes = { GrantType.AuthorizationCode },

                RedirectUris = {
                    "https://localhost:44300/signin-oidc",
                    "http://localhost:5005/appointments",
                    "http://localhost:5005/swagger/oauth2-redirect.html",
                    "http://127.0.0.1:5005/swagger/oauth2-redirect.html"
                },

                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "documents" }
            },
        };

        public static List<ApiResource> ApiResources = new List<ApiResource>
        {
            //new ApiResource
            //{
            //    Name = "identity-server-demo-api",
            //    DisplayName = "Identity Server Demo API",
            //    Scopes = new List<string>
            //    {
            //        "write",
            //        "read"
            //    }
            //}
        };

        public static IEnumerable<ApiScope> ApiScopes = new List<ApiScope>
        {
            new ApiScope("offices"),
            new ApiScope("profiles"),
            new ApiScope("services"),
            new ApiScope("appointments"),
            new ApiScope("documents"),
        };

        public static IEnumerable<IdentityRole> IdentityRoles =>
            new IdentityRole[]
            {
                new IdentityRole("receptionist"),
                new IdentityRole("patient"),
                new IdentityRole("doctor"),
                new IdentityRole("admin"),
            };
    }
}
