using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Dex.Extensions;
using Dex.MassTransit.Rabbit;
using Identity.Consumers;
using Identity.Mapping;
using Identity.Options;
using Identity.Services;
using Shared;
using Grpc.Health.V1;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared.Commands;

namespace Identity
{
    public class Startup : BaseStartup
    {
        private const string RootPath = "/identity";

        public Startup(IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // configuration
            services.Configure<GrpcClientsOptions>(Configuration.GetSection(nameof(GrpcClientsOptions)).Bind);
            services.Configure<IdentityOptions>(options => options.ProviderName = "logistic");

            services.AddControllers();

            // grpc services
            ConfigureGrpcServices(services);

            // identity
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseInformationEvents = false;
                    options.Events.RaiseSuccessEvents = false;

                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = false;
                })
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(connectionString, optionsBuilder =>
                        {
                            optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName);
                            optionsBuilder.EnableRetryOnFailure();
                        });
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(connectionString,
                            optionsBuilder =>
                            {
                                optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName);
                                optionsBuilder.EnableRetryOnFailure();
                            });

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600;
                })
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddConfigurationStoreCache();

            // Signing Key
            var signTokenKeyFileName = Environment.IsProduction() ? "prod-sign-token-key.json" : "sign-token-key.json";
            var rsa = RSA.Create(JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(signTokenKeyFileName)));
            builder.AddSigningCredential(new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256));

            services.AddAuthentication();

            // automapper
            services.AddAutoMapper(expression =>
            {
                expression.AddMaps(Assembly.GetEntryAssembly());
                expression.AddProfile(new MainProfile());
            });

            //masstransit
            var concurrencyLimit = Environment.IsDevelopment() ? 1 : System.Environment.ProcessorCount;

            var internalMqOptions = new RabbitMqOptions();
            Configuration.GetSection(nameof(RabbitMqOptions)).Bind(internalMqOptions);
            services.AddMassTransit(x =>
            {
                x.AddConsumer<InvalidateUserTokenConsumer>(configurator =>
                {
                    configurator.UseConcurrencyLimit(concurrencyLimit);
                    configurator.UseMessageRetry(retryConfigurator => retryConfigurator.Interval(100, 10.Seconds()));
                });

                x.RegisterBus((context, configurator) =>
                {
                    context.RegisterReceiveEndpoint<InvalidateUserTokenConsumer, UserTokenCommand>(configurator, rabbitMqOptions: internalMqOptions);
                }, internalMqOptions);
            });

            // services
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddScoped<IInvalidateUserTokenService, InvalidateUserTokenService>();
        }

        private static void ConfigureGrpcServices(IServiceCollection services)
        {
            services.AddGrpcClient<UserStore.UserStoreClient>("web.client.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.ProfileServiceUrl); // такойже как и мобайл, аутентификация там же
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "web.client.userstore" };
            });
            services.AddGrpcClient<UserStore.UserStoreClient>("mobile.client.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.ProfileServiceUrl);
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "mobile.client.userstore" };
            });
            services.AddGrpcClient<UserStore.UserStoreClient>("mobile.client.long.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.ProfileServiceUrl);
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "mobile.client.long.userstore" };
            });
            services.AddGrpcClient<UserStore.UserStoreClient>("admin.client.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.AdminServiceUrl);
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "admin.client.userstore" };
            });

            // grpc health checks
            services.AddGrpcClient<Health.HealthClient>("mobile.client.health", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.ProfileServiceUrl);
            });
            services.AddGrpcClient<Health.HealthClient>("admin.client.health", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientsOptions>>();
                o.Address = new Uri(options.Value.AdminServiceUrl);
            });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString(RootPath)); // for proxy, remove prefix from request (/identity/get == /get)

            if (env.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }

            app.UseStaticFiles();

            app.UseIdentityServer();
            app.UseRouting();
            app.UseAuthentication();

            app.UseEndpoints(builder =>
            {
                ConfigureSystemEndpoints(builder);
                builder.MapDefaultControllerRoute();
            });
        }
    }
}