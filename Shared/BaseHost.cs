using System;
using System.Globalization;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shared.Logger;

namespace Shared
{

    public abstract class BaseHost<TStartup> where TStartup : class
    {
        public async Task<int> Run(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Information("Creating host...");
                var host = CreateHostBuilder(args).Build();
                Log.Information("Starting host...");
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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


        private IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(
                        $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.local.json",
                        optional: true, reloadOnChange: false);
                })
                .ConfigureLogger()
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
            kestrelServerOptions.ConfigureHttpsDefaults(opt =>
            {
                opt.SslProtocols = SslProtocols.None; // can OS autoselect protocol
            });

            var urls = hostBuilderContext.Configuration["ASPNETCORE_URLS"]?
                .Replace("+", "host.hh", StringComparison.Ordinal)
                .Split(";");

            if (urls != null)
            {
                foreach (var url in urls)
                {
                    var uri = new Uri(url);
                    if (uri.IsLoopback)
                    {
                        kestrelServerOptions.ListenLocalhost(uri.Port, options => { EnableHttpsIfNeed(options, uri); });
                    }
                    else
                    {
                        kestrelServerOptions.ListenAnyIP(uri.Port, options => { EnableHttpsIfNeed(options, uri); });
                    }
                }
            }


            void EnableHttpsIfNeed(ListenOptions options, Uri uri)
            {
                if (uri.IsAbsoluteUri && uri.Scheme.ToLower(CultureInfo.InvariantCulture) == "https")
                {
                    InitHttps(options, hostBuilderContext);
                }
            }
        }
        
        protected virtual Task SeedDatabase(IHost host, bool ensureDeleted)
        {
            return Task.CompletedTask;
        }

        protected void InitHttps(ListenOptions options, WebHostBuilderContext hostBuilderContext)
        {
            if (hostBuilderContext == null) throw new ArgumentNullException(nameof(hostBuilderContext));

            var certPath = hostBuilderContext.Configuration["ASPNETCORE_Kestrel:Certificates:Default:Path"];
            var isCertFound = File.Exists(certPath);
            if (isCertFound)
            {
                var certPassword = hostBuilderContext.Configuration["ASPNETCORE_Kestrel:Certificates:Default:Password"];
#pragma warning disable CA2000
                options.UseHttps(new X509Certificate2(certPath, certPassword));
#pragma warning restore CA2000
            }
            else
            {
                options.UseHttps();
            }
        }
    }
}