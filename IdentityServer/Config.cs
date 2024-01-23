// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("offices.edit"),
                new ApiScope("patients.edit"),
                new ApiScope("doctors.edit"),
                new ApiScope("receptionists.edit"),
                new ApiScope("services.edit"),
                new ApiScope("specializations.edit"),
            };

        public static IEnumerable<IdentityRole> IdentityRoles =>
            new IdentityRole[]
            {
                new IdentityRole("receptionist"),
                new IdentityRole("patient"),
                new IdentityRole("doctor"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
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
                    AllowedScopes = { "offices.edit" }
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
                    AllowedScopes = { "patients.edit", "receptionists.edit", "doctors.edit" }
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
                    AllowedScopes = { "services.edit", "specializations.edit" }
                },
            };
    }
}