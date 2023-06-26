using System;
using Dex.Cap.Outbox.AspNetScheduler;
using Dex.Cap.Outbox.Ef;
using Dex.Cap.Outbox.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Profile.Dal.Domain;
using Profile.Dal.Repositories;
using Shared.Dal;
using Shared.Dal.Extensions;

namespace Profile.Dal;

public static class MicrosoftDependencyInjectionExtensions
{
    public static void RegisterDal(this IServiceCollection services, string connectionString)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddDbContext<ReadProfileDbContext>(connectionString, noTracking: true);
        services.AddScoped<IReadDbContext>(p => p.GetRequiredService<ReadProfileDbContext>());
        services.AddScoped<IReadProfileDbContext>(p => p.GetRequiredService<ReadProfileDbContext>());

        // dbContexts
        services.AddDbContext<ProfileDbContext>(connectionString, noTracking: false);
        services.AddScoped<IUnityOfWork>(p => p.GetRequiredService<ProfileDbContext>());
        services.AddScoped<IWriteDbContext>(p => p.GetRequiredService<ProfileDbContext>());

        // repositories
        services.RegisterGenericRepository();

        services.AddScoped<IWriteUserRepository, WriteUserRepository>();
        services.AddScoped<IReadUserRepository, ReadUserRepository>();
        
        services.AddScoped<IWriteNotificationRepository, WriteNotificationRepository>();
        services.AddScoped<IReadNotificationRepository, ReadNotificationRepository>();
        
        // outbox
        services.AddOutbox<ProfileDbContext>();
        services.RegisterOutboxScheduler(5);
        services.AddScoped<IOutboxService<IUnityOfWork>>(provider => provider.GetRequiredService<IOutboxService<ProfileDbContext>>());
    }
}