using System;
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

        services.AddScoped<IReadUserRepository, ReadUserRepository>();
    }
}