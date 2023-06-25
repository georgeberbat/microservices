using Dex.Cap.Outbox.AspNetScheduler;
using Dex.Cap.Outbox.Ef;
using Dex.Cap.Outbox.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Location.Dal.Domain;
using Location.Dal.Repositories;
using Shared.Dal;
using Shared.Dal.Extensions;

namespace Location.Dal
{
    public static partial class MicrosoftDependencyInjectionExtensions
    {
        public static void RegisterDal(this IServiceCollection services, string connectionString)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<ReadLocationDbContext>(connectionString, noTracking: true);
            services.AddScoped<IReadDbContext>(p => p.GetRequiredService<ReadLocationDbContext>());
            services.AddScoped<IReadLocationDbContext>(p => p.GetRequiredService<ReadLocationDbContext>());

            // dbContexts
            services.AddDbContext<LocationDbContext>(connectionString, noTracking: false);
            services.AddScoped<IUnityOfWork>(p => p.GetRequiredService<LocationDbContext>());
            services.AddScoped<IWriteDbContext>(p => p.GetRequiredService<LocationDbContext>());

            // repositories
            services.RegisterGenericRepository();

            services.AddScoped<IWriteLocationRepository, WriteLocationRepository>();
            services.AddScoped<IReadLocationRepository, ReadLocationRepository>();

            // outbox
            services.AddOutbox<LocationDbContext>();
            services.RegisterOutboxScheduler(5);
            services.AddScoped<IOutboxService<IUnityOfWork>>(provider =>
                provider.GetRequiredService<IOutboxService<LocationDbContext>>());
        }
    }
}

namespace Location.Dal.Public
{
    public static partial class MicrosoftDependencyInjectionExtensions
    {
        public static void RegisterLocationReadonlyDal(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ReadLocationDbContext>(connectionString, noTracking: true);
            services.AddScoped<IReadLocationRepository, ReadLocationRepository>();
        }
    }
}