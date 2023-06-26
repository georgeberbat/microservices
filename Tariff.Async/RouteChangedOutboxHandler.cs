using Dex.Cap.Outbox.Interfaces;
using MassTransit;
using ProfileDomain.Commands;

namespace Tariff.Async;

public class RouteChangedOutboxHandler : IOutboxMessageHandler<RouteChangedCommand>
{
    private readonly ISendEndpointProvider _sendEndpoint;

    public RouteChangedOutboxHandler(ISendEndpointProvider sendEndpoint)
    {
        _sendEndpoint = sendEndpoint ?? throw new ArgumentNullException(nameof(sendEndpoint));
    }

    public async Task ProcessMessage(RouteChangedCommand message, CancellationToken cancellationToken)
    {
        await _sendEndpoint.Send(message, cancellationToken);
    }

    public Task ProcessMessage(IOutboxMessage outbox, CancellationToken cancellationToken)
    {
        return ProcessMessage((RouteChangedCommand)outbox, cancellationToken);
    }
}