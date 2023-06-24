using Microsoft.Extensions.DependencyInjection;

namespace Tariff.Services;

public static class MicrosoftDependencyInjectionExtensions
{
    public static void RegisterInternalServices(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<ITariffService, TariffService>();
    }
}