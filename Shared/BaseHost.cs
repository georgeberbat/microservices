using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Common.Logging;
using Common.Logging.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Shared
{
    [SuppressMessage("Globalization", "CA1305:Укажите IFormatProvider")]
    public abstract class BaseHost<TStartup> where TStartup : class
    {
        public async Task<int> Run(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                const string seedKey = "--seed";
                const string runAsWindowsServiceKey = "--runAsWindowsService";

                var seed = args.Contains(seedKey);
                if (seed)
                {
                    args = args.Except(new[] { seedKey }).ToArray();
                }

                Log.Information("Creating host...");

                IHost host;
                if (args.Any(x => x[..].Contains(runAsWindowsServiceKey, StringComparison.OrdinalIgnoreCase)))
                {
                    var split = Array.Empty<string>();
                    foreach (var t in args)
                    {
                        if (t.Contains(runAsWindowsServiceKey, StringComparison.OrdinalIgnoreCase))
                        {
                            split = t.Split(' ');
                        }
                    }

                    host = CreateHostBuilder(args)
                        .ConfigureAppConfiguration(conf =>
                        {
                            conf.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                        })
                        .Build();
                }
                else
                {
                    host = CreateHostBuilder(args)
                        .Build();
                }

                if (seed)
                {
                    Log.Information("Migration and seeding...");
                    await RunSeeder(args, host);
                }
                else
                {
                    Log.Information("Starting host...");
                    await host.RunAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
#if DEBUG
                throw;
#else
                return 1;
#endif
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return 0;
        }

        public async Task<int> RunAsWindowsService(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            if (args.Length == 0)
            {
                args = new[] { "--runAsWindowsService GorodPayService" };
            }
            else if (!args.Contains("--runAsWindowsService"))
            {
                var array = new string[args.Length + 1];
                for (var i = 0; i < args.Length; i++)
                {
                    array[i] = args[i];
                }

                array[^1] = "--runAsWindowsService GorodPayService";
                args = array;
            }

            return await Run(args);
        }

        private async Task RunSeeder(string[] args, IHost host)
        {
            const string dropKey = "--drop";
            const string forceKey = "--force";

            var env = host.Services.GetRequiredService<IHostEnvironment>();

            var dropDb = args.Contains(dropKey);
            var force = args.Contains(forceKey);
            var ensureDeleted = dropDb && (!env.IsProduction() || env.IsProduction() && force);

            Log.Information(
                "Migrating database: [{Environment}], drop: {Drop}, force {Force}, ensureDeleted: {EnsureDeleted}",
                env.EnvironmentName, dropDb, force, ensureDeleted);

            await SeedDatabase(host, ensureDeleted);

            Log.Information("Migrating database completed");
        }

        protected virtual Task SeedDatabase(IHost host, bool ensureDeleted)
        {
            return Task.CompletedTask;
        }

        private IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    configurationBuilder.Sources.Clear();

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, false)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true,
                            false)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                        .Build();

                    configurationBuilder.AddConfiguration(configuration);
                })
                .UseSerilog((context, configuration) =>
                {
                    // serilog
                    configuration.ReadFrom.Configuration(context.Configuration);
                    configuration.Enrich.FromLogContext();
                    configuration.Enrich.WithSpan();
                    configuration.Enrich.With<RemovePropertiesEnricher>();
                    LogManager.Adapter = new SerilogFactoryAdapter();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(ConfigureKestrel);
                    webBuilder.UseStartup<TStartup>();
                });

        protected virtual void ConfigureKestrel(WebHostBuilderContext hostBuilderContext,
            KestrelServerOptions kestrelServerOptions)
        {
            if (hostBuilderContext == null) throw new ArgumentNullException(nameof(hostBuilderContext));
            if (kestrelServerOptions == null) throw new ArgumentNullException(nameof(kestrelServerOptions));

            kestrelServerOptions.AllowSynchronousIO = true;
#pragma warning disable CA5398
            kestrelServerOptions.ConfigureHttpsDefaults(opt =>
            {
                opt.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });
#pragma warning restore CA5398

            var certPath = hostBuilderContext.Configuration["ASPNETCORE_Kestrel:Certificates:Default:Path"];
            var certPassword = hostBuilderContext.Configuration["ASPNETCORE_Kestrel:Certificates:Default:Password"];
            var isCertFound = File.Exists(certPath);
#pragma warning disable CA2000
            var cert = isCertFound ? new X509Certificate2(certPath, certPassword) : null;
#pragma warning restore CA2000
            var urls = hostBuilderContext.Configuration["ASPNETCORE_URLS"]
                ?.Replace("+", "host.hh", StringComparison.OrdinalIgnoreCase).Split(";");
            if (urls != null)
            {
                foreach (var url in urls)
                {
                    var uri = new Uri(url);
                    if (uri.IsLoopback)
                    {
                        kestrelServerOptions.ListenLocalhost(uri.Port, options => EnableHttpsIfNeed(options, uri));
                    }
                    else
                    {
                        kestrelServerOptions.ListenAnyIP(uri.Port, options => EnableHttpsIfNeed(options, uri));
                    }
                }
            }

            if (int.TryParse(hostBuilderContext.Configuration["Grpc:Port"], out var port))
            {
                kestrelServerOptions.ListenAnyIP(port, options =>
                {
                    options.Protocols = HttpProtocols.Http2;
                    InitHttps(options);
                });
            }

            void EnableHttpsIfNeed(ListenOptions options, Uri uri)
            {
                if (uri.IsAbsoluteUri && uri.Scheme.ToLower(CultureInfo.InvariantCulture) == "https")
                {
                    InitHttps(options);
                }
            }

            void InitHttps(ListenOptions options)
            {
                if (isCertFound)
                {
                    options.UseHttps(cert!);
                }
                else
                {
                    options.UseHttps();
                }
            }
        }

        private class RemovePropertiesEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
            {
                le.RemovePropertyIfPresent("metadata.detail");
            }
        }
    }
}