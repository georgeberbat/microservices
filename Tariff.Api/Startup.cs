﻿using gRPC.UserStore.Exception;
using Tariff.Dal;
using Location.Dal.Public;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Extensions;
using Tariff.Async;
using Tariff.Services;

namespace Tariff.Api
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
            
            var locationConnectionString = Configuration.GetConnectionString("LocationConnection");
            services.RegisterLocationReadonlyDal(locationConnectionString);

            services.RegisterInternalServices();
            services.RegisterAsyncServices(Configuration);

            // grpc
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ExceptionInterceptor>();
            });

            // settings
            services.AddOptionsWithDataAnnotationsValidation<TokenOptions>(
                Configuration.GetSection(nameof(TokenOptions)));
            services.AddMvc();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString("/Tariff"));

            base.Configure(app, env);
        }

        protected override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.ConfigureEndpoints(endpoints);

            var grpEndpoint = $"*:{Configuration["Grpc:Port"]}";
            endpoints.MapGrpcService<TariffGrpcService>().RequireHost(grpEndpoint);
            endpoints.MapGrpcService<RouteGrpcService>().RequireHost(grpEndpoint);
        }
    }
}