using Dex.Cap.Outbox.AspNetScheduler;
using Dex.Cap.Outbox.Ef;
using Dex.Cap.Outbox.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Dal;
using Shared.Dal.Extensions;
using Tariff.Dal.Domain;
using Tariff.Dal.Repositories;

namespace Tariff.Dal;

public static class MicrosoftDependencyInjectionExtensions
{
    public static void RegisterDal(this IServiceCollection services, string connectionString)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddDbContext<ReadTariffDbContext>(connectionString, noTracking: true);
        services.AddScoped<IReadDbContext>(p => p.GetRequiredService<ReadTariffDbContext>());
        services.AddScoped<IReadTariffDbContext>(p => p.GetRequiredService<ReadTariffDbContext>());

        // dbContexts
        services.AddDbContext<TariffDbContext>(connectionString, noTracking: false);
        services.AddScoped<IUnityOfWork>(p => p.GetRequiredService<TariffDbContext>());
        services.AddScoped<IWriteDbContext>(p => p.GetRequiredService<TariffDbContext>());

        // repositories
        services.RegisterGenericRepository();

        services.AddScoped<IWriteTariffRepository, WriteTariffRepository>();
        services.AddScoped<IReadTariffRepository, ReadTariffRepository>();
        
        // outbox
        services.AddOutbox<TariffDbContext>();
        services.RegisterOutboxScheduler(5);
        services.AddScoped<IOutboxService<IUnityOfWork>>(provider => provider.GetRequiredService<IOutboxService<TariffDbContext>>());
    }
}