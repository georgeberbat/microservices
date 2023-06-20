// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace Identity.SeedData
{
    public static class SeedConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile
                {
                    UserClaims = new List<string>
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Picture,
                    }
                },
                new("policy", new[] {"policy"})
            };

        public static IEnumerable<ApiResource> ApiResources => new[]
        {
            new ApiResource("s.profile", "Profile service API")
            {
                Scopes = new List<string> {"composition-api"}
            },
            new ApiResource("s.composition", "Composition service API")
            {
                Scopes = new List<string> {"composition-api"},
                ApiSecrets = {new Secret("B5F5CFD9-042A-44A1-8541-4140D97DF455".Sha512())}
            }
        };

        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("composition-api")
        };

        public static IEnumerable<Client> Clients => new[]
        {
            new Client
            {
                ClientName = "Composition API",
                ClientSecrets = {new Secret("B911A480-960E-43BA-BF52-DEC5FFCC1679".Sha256())},
                ClientId = "composition.client",
                AccessTokenType = AccessTokenType.Jwt,
                
                // AllowedCorsOrigins = new List<string>
                // {
                //     "https://localhost"
                // },
                
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {"openid", "profile", "composition-api"},

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 8 * 3600, // 8 час
                AbsoluteRefreshTokenLifetime = 365 * 24 * 3600, // 365 дней
                SlidingRefreshTokenLifetime = 30 * 24 * 3600, // 30 дней
            }
        };
    }
}