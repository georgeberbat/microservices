using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Shared.Dal.Interceptors;

namespace Shared.Dal.Extensions
{
    public static class MicrosoftDependencyInjectionExtensions
    {
        public static void RegisterGenericRepository(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped(typeof(IReadRepository<,>), typeof(GenericReadRepository<,>));
            services.AddScoped(typeof(IWriteRepository<,>), typeof(GenericWriteRepository<,>));
            services.AddScoped(typeof(IWriteRepository<,,>), typeof(GenericWriteRepository<,,>));
        }

        public static void AddDbContext<T>(this IServiceCollection services, string connectionString,
            int maxRetryCount = 3, bool noTracking = false, TimeSpan maxRetryDelay = default, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
            where T : DbContext
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            RegisterRequiredServices<T>(services);

            services.AddDbContext<T>((sp, builder) =>
            {
                builder.UseNpgsql(connectionString, optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly(typeof(T).Assembly.GetName().Name);
                    optionsBuilder.EnableRetryOnFailure(maxRetryCount,
                        maxRetryDelay != default ? maxRetryDelay : TimeSpan.FromMilliseconds(500), null);

                    npgsqlOptionsAction?.Invoke(optionsBuilder);
                });

                if (noTracking)
                {
                    builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }
                else
                {
                    builder.AddInterceptors(sp.GetServices<IInterceptor>());
                }
            });
        }

        private static void RegisterRequiredServices<T>(IServiceCollection services) where T : DbContext
        {
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddSingleton(NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator);
            services.AddSingleton<ModelStore<T>>();

            services.AddSingleton<IInterceptor, EvaluateAutoDatetimeColumnInterceptor>();
            services.AddScoped<IInterceptor, EntityChangesTriggerInterceptor>();
        }
    }
}