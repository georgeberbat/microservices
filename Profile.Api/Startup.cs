using System;
using IdentityModel.AspNetCore.AccessTokenValidation;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profile.Dal;
using Profile.Dal.Migrations;
using Profile.ServiceInterfaces.Repositories;
using Profile.ServiceInterfaces.Services;
using Profile.Services;
using Shared;
using Shared.Dal;
using Shared.Extensions;
using Shared.Options;

namespace Profile
{

    public class Startup : BaseStartup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ProfileDbContext, IProfileMigrationMarker>(connectionString);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            // settings
            services.AddOptionsWithDataAnnotationsValidation<TokenOptions>(Configuration.GetSection(nameof(TokenOptions)));
            services.AddMvc();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString("/profile")); // for proxy, remove prefix from request (/profile/get == /get)

            base.Configure(app, env);
        }

        protected override void ConfigureAuthorization(AuthorizationSettings authorizationSettings, AuthorizationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            base.ConfigureAuthorization(authorizationSettings, options);

            options.AddPolicy("device-regular_user",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireScope("device-api", "mobile-api");
                });
        }
    }
}