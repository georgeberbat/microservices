using ApiComposition.Api.GrpcClients;
using ApiComposition.Api.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Options;
using Shared.Services;
using BaseStartup = Shared.BaseStartup;

namespace ApiComposition.Api
{

    public class Startup : BaseStartup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.Configure<GrpcClientsOptions>(Configuration.GetSection(nameof(GrpcClientsOptions)).Bind);
            
            // settings
            services.AddOptionsWithDataAnnotationsValidation<TokenOptions>(Configuration.GetSection(nameof(TokenOptions)));
            services.AddMvc();
            
            //grpc
            services.AddGrpcClient<ProfileGrpc.ProfileGrpcClient>("composition.client.profilegrpc", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.ProfileServiceUrl);
                o.Creator = invoker => new ProfileGrpc.ProfileGrpcClient(invoker) { Name = "composition.client.profilegrpc" };
            });
            
            
            services.AddGrpcClient<LocationGrpc.LocationGrpcClient>("composition.client.locationgrpc", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.LocationServiceUrl);
                o.Creator = invoker => new LocationGrpc.LocationGrpcClient(invoker) { Name = "composition.client.locationgrpc" };
            });
            
            services.AddScoped<ProfileClient>();
            services.AddScoped<LocationClient>();
            services.AddScoped<IUserIdHttpContextService, UserIdHttpContextService>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString("/api-composition")); // for proxy, remove prefix from request (/profile/get == /get)

            base.Configure(app, env);
        }

        protected override void ConfigureAuthorization(AuthorizationSettings authorizationSettings, AuthorizationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            base.ConfigureAuthorization(authorizationSettings, options);

            // options.AddPolicy("device-regular_user",
            //     policy =>
            //     {
            //         policy.RequireAuthenticatedUser();
            //         policy.RequireScope("composition-api");
            //     });
        }
    }
}