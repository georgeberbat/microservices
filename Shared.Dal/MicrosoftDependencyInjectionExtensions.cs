using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Npgsql;


namespace Shared.Dal
{
    public static class MicrosoftDependencyInjectionExtensions
    {
        private const int DefaultRetryDelay = 500;

        public static void AddDbContext<T, TR>(this IServiceCollection services, string connectionString,
            int maxRetryCount = 3, TimeSpan maxRetryDelay = default)
            where T : DbContext
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            RegisterRequiredServices<T>(services);
            
            services.AddDbContext<T>(builder =>
            {
                builder.UseNpgsql(connectionString, optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly(typeof(TR).Assembly.GetName().Name);
                    optionsBuilder.EnableRetryOnFailure(maxRetryCount,
                        maxRetryDelay != default ? maxRetryDelay : TimeSpan.FromMilliseconds(DefaultRetryDelay), null);
                });
            });
        }

        public static void AddReadDbContext<T>(this IServiceCollection services, string connectionString,
            int maxRetryCount = 3, TimeSpan maxRetryDelay = default)
            where T : DbContext
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            RegisterRequiredServices<T>(services);

            services.AddDbContext<T>(builder =>
            {
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                builder.UseNpgsql(connectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.EnableRetryOnFailure(maxRetryCount,
                            maxRetryDelay != default ? maxRetryDelay : TimeSpan.FromMilliseconds(DefaultRetryDelay), null);
                    });
            });
        }

        private static void RegisterRequiredServices<T>(IServiceCollection services) where T : DbContext
        {
            services.AddSingleton<ModelStore<T>, ModelStore<T>>();
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddSingleton(NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator);
        }
    }
}