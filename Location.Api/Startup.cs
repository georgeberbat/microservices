using IdentityModel.AspNetCore.AccessTokenValidation;
using Location.Dal;
using Location.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Extensions;
using Shared.Options;

namespace Location.Api
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
            services.RegisterDal(connectionString);

            services.RegisterInternalServices();
            // settings
            services.AddOptionsWithDataAnnotationsValidation<TokenOptions>(
                Configuration.GetSection(nameof(TokenOptions)));
            services.AddMvc();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString("/location"));

            base.Configure(app, env);
        }

        protected override void ConfigureAuthorization(AuthorizationSettings authorizationSettings,
            AuthorizationOptions options)
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
        
        protected override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.ConfigureEndpoints(endpoints);

            var grpEndpoint = $"*:{Configuration["Grpc:Port"]}";
            endpoints.MapGrpcService<LocationGrpcService>().RequireHost(grpEndpoint);
        }
    }
}