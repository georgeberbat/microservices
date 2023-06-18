using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Shared.BaseDbSeeder;
using Shared.BaseDbSeeder.Seeder;

namespace Shared.Dal
{
    public static class SeedRunner
    {
        public static async Task Seed<TStartup, TSeeder>(string[] args,
            IConfigurationRoot? externalConfiguration = default)
            where TStartup : BaseStartup
            where TSeeder : IDbSeeder
        {
            Log.Information("Build services...");

            var hostBuilder = Host.CreateDefaultBuilder(args);

            if (externalConfiguration != default)
            {
                hostBuilder.ConfigureAppConfiguration(builder => builder.AddConfiguration(externalConfiguration));
            }

            var host = hostBuilder
                .ConfigureLogger()
                .ConfigureServices((context, collection) =>
                    CreateStartup<TStartup>(context).ConfigureServices(collection))
                .Build();


            Log.Information("Build services completed");

            const string dropKey = "--drop";
            const string forceKey = "--force";

            try
            {
                var dropDb = args.Contains(dropKey);
                var force = args.Contains(forceKey);

                var env = host.Services.GetRequiredService<IHostEnvironment>();
                var ensureDeleted = dropDb && (!env.IsProduction() || env.IsProduction() && force);

                Log.Information(
                    "Migrating database: [{Environment}], drop: {Drop}, force {Force}, ensureDeleted: {EnsureDeleted}",
                    env.EnvironmentName, dropDb, force, ensureDeleted);

                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var seeder = serviceProvider.GetRequiredService<TSeeder>();
                    await seeder.RunAsync(ensureDeleted).ConfigureAwait(false);
                    Log.Information("Migrating database completed");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Migrator terminated unexpectedly");
                Environment.ExitCode = -1;
            }

            Log.CloseAndFlush();
        }

        private static BaseStartup CreateStartup<TStartup>(HostBuilderContext context) where TStartup : BaseStartup
        {
            var ctor = typeof(TStartup).GetConstructor(new[] { typeof(HostBuilderContext) });

            if (ctor == null)
                throw new InvalidOperationException(
                    "Startup constructor with signature ctor(HostBuilderContext), not found");

            return (BaseStartup)ctor.Invoke(new object[] { context });
        }

        private static IHostBuilder ConfigureLogger(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.Enrich.WithProperty("AppName", AppDomain.CurrentDomain.FriendlyName);
            });
        }
    }
}