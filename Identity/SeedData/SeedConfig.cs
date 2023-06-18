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
                Scopes = new List<string> {"mobile-api", "webclient-api", "partner-api", "device-api"}
            },
            new ApiResource("s.mobile", "Mobile service API")
            {
                Scopes = new List<string> {"mobile-api", "webclient-api"}
            },
            new ApiResource("s.admin", "Admin API")
            {
                Scopes = new List<string> {"admin-api"},
                UserClaims = new List<string> {"policy"},
                ApiSecrets = {new Secret("53193185-D96E-462F-A920-1CBE6B15EB4D".Sha512())}
            },
            new ApiResource("s.transportOnline", "TransportOnline service API")
            {
                Scopes = new List<string> {"mobile-api", "webclient-api"}
            },
            new ApiResource("s.contentManagement", "ContentManagement service API")
            {
                Scopes = new List<string> {"admin-api"},
                UserClaims = new List<string> {"policy"},
                ApiSecrets = {new Secret("B5F5CFD9-042A-44A1-8541-4140D97DF455".Sha512())}
            },
            new ApiResource("s.notificationCenter", "Notification center service API")
            {
                Scopes = new List<string> {"mobile-api", "admin-api"},
                UserClaims = new List<string> {"policy"},
                ApiSecrets = {new Secret("9906054D-A35E-4321-ADF0-8640A7BDFCCB".Sha512())}
            },
        };

        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("mobile-api"),
            new ApiScope("webclient-api"),
            new ApiScope("admin-api"),
            new ApiScope("partner-api"),
            new ApiScope("device-api"),
        };

        public static IEnumerable<Client> Clients => new[]
        {
            new Client
            {
                ClientName = "Mobile device auth",
                ClientId = "mobile.device",
                ClientSecrets = {new Secret("C775BA15-30C5-4DAB-86D4-160341F3EBA4".Sha256())},
                AccessTokenType = AccessTokenType.Jwt,

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {"device-api"},
                AccessTokenLifetime = 10 * 60, // 10 min
            },

            new Client
            {
                ClientName = "Partner Test1, external",
                ClientId = "external.TestClient1",
                ClientSecrets = {new Secret("9D910006-59DA-4913-A0F0-303519F2616E".Sha256())},
                AccessTokenType = AccessTokenType.Jwt,

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {"partner-api"},

                AccessTokenLifetime = 1 * 3600, // 1 hours
            },

            new Client
            {
                ClientName = "Mobile API",
                ClientSecrets = {new Secret("B911A480-960E-43BA-BF52-DEC5FFCC1679".Sha256())},
                ClientId = "mobile.client",
                AccessTokenType = AccessTokenType.Jwt,

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {"openid", "profile", "mobile-api"},

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 8 * 3600, // 8 час
                AbsoluteRefreshTokenLifetime = 365 * 24 * 3600, // 365 дней
                SlidingRefreshTokenLifetime = 30 * 24 * 3600, // 30 дней
            },

            new Client
            {
                ClientName = "Mobile API long life",
                ClientSecrets = {new Secret("06611CCA-2606-4A01-9821-610818FB8ABA".Sha256())},
                ClientId = "mobile.client.long",
                AccessTokenType = AccessTokenType.Jwt,

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {"openid", "profile", "mobile-api"},

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 8 * 3600, // 8 час
                AbsoluteRefreshTokenLifetime = 5 * 365 * 24 * 3600, // 5 лет
                SlidingRefreshTokenLifetime = 180 * 24 * 3600, // 180 дней
            },

            new Client
            {
                ClientName = "WebCabinet API",
                ClientSecrets = {new Secret("6E664467-1EC1-4465-B608-57CD07109B1E".Sha256())},
                ClientId = "web.client",
                AccessTokenType = AccessTokenType.Jwt,
                AllowedCorsOrigins = new List<string>
                {
                    "https://web.gorpay.online",
                    "http://gp-lk.amm-c.ru",
                    "https://lk-test.ru",
                    "https://lk.ru"
                },

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {"openid", "profile", "webclient-api"},

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 3600, // 1 hours
                AbsoluteRefreshTokenLifetime = 30 * 24 * 3600, // 30 days
                SlidingRefreshTokenLifetime = 7 * 24 * 3600 // 7 days
            },

            new Client
            {
                ClientName = "Admin API",
                ClientSecrets = {new Secret("1313BFC3-CF8B-4F17-A195-E18D0C4D2690".Sha256())},
                ClientId = "admin.client",
                AccessTokenType = AccessTokenType.Reference,
                AllowedCorsOrigins = new List<string>
                {
                    "https://admin.gorpay.online",
                    "https://admin-dev.gorpay.online",
                    "http://localhost.admin-dev.gorpay.online:3000",
                    "https://admin-pp.gorpay.online",
                    "https://admin-stage.gorpay.online"
                },

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {"openid", "profile", "admin-api", "policy"},

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 300,
                AbsoluteRefreshTokenLifetime = 1 * 3600,
                SlidingRefreshTokenLifetime = 3600
            },

            // // interactive client using code flow + pkce
            // new Client
            // {
            //     ClientId = "interactive",
            //     ClientName = "Interactive client",
            //     ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},
            //
            //     AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            //
            //     RedirectUris = {"https://localhost:44300/signin-oidc"},
            //     FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
            //     PostLogoutRedirectUris = {"https://localhost:44300/signout-callback-oidc"},
            //
            //     AllowOfflineAccess = true,
            //     AllowedScopes = {"openid", "profile", "scope2"}
            // },
        };
    }
}