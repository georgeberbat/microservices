using System;
using Dex.Extensions;
using Dex.MassTransit.Rabbit;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profile.Async;
using Profile.Dal;
using Profile.Services;
using ProfileDomain.Commands;
using Shared.Extensions;
using Shared.Options;
using Shared.Password;
using BaseStartup = Shared.BaseStartup;

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

            services.AddOptionsWithDataAnnotationsValidation<TokenOptions>(
                Configuration.GetSection(nameof(TokenOptions)));
            services.AddMvc();

            // grpc
            services.AddGrpc();

            services.AddScoped<IPasswordGenerator, PasswordGenerator>();
            services.AddScoped<IUserService, UserService>();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.RegisterDal(connectionString);
            
            // masstransit
            services.AddMassTransit(x =>
            {
                x.AddConsumer<RouteChangedConsumer>(configurator =>
                    {
                        configurator.UseConcurrencyLimit(1);
                        configurator.UseMessageRetry(retryConfigurator => retryConfigurator.Interval(100, 10.Seconds()));
                    }
                );

                x.RegisterBus((context, configurator) =>
                {
                    context.RegisterReceiveEndpoint<RouteChangedConsumer, RouteChangedCommand>(configurator);
                });
            });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString("/profile")); // for proxy, remove prefix from request (/profile/get == /get)

            base.Configure(app, env);
        }

        protected override void ConfigureAuthorization(AuthorizationSettings authorizationSettings,
            AuthorizationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            base.ConfigureAuthorization(authorizationSettings, options);
        }

        protected override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.ConfigureEndpoints(endpoints);

            var grpEndpoint = $"*:{Configuration["Grpc:Port"]}";
            endpoints.MapGrpcService<UserStoreService>().RequireHost(grpEndpoint);
            endpoints.MapGrpcService<ProfileGrpcService>().RequireHost(grpEndpoint);
        }
    }
}