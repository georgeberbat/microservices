using Dex.Cap.Outbox.Interfaces;
using Dex.Extensions;
using Dex.MassTransit.Rabbit;
using Location.Models.Commands;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProfileDomain.Commands;

namespace Tariff.Async;

public static class MicrosoftDependencyInjectionExtensions
{
    public static void RegisterAsyncServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)).Bind);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OnLocationRemovedConsumer>(configurator =>
                {
                    configurator.UseConcurrencyLimit(1);
                    configurator.UseMessageRetry(retryConfigurator => retryConfigurator.Interval(100, 10.Seconds()));
                }
            );

            x.RegisterBus((context, configurator) =>
            {
                context.RegisterReceiveEndpoint<OnLocationRemovedConsumer, OnLocationRemovedCommand>(configurator);
                
                context.RegisterSendEndPoint<RouteChangedCommand>();
            });
        });
        
        services.AddScoped<IOutboxMessageHandler<RouteChangedCommand>, RouteChangedOutboxHandler>();
    }
}