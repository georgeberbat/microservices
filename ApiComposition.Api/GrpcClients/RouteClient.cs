using Google.Protobuf.WellKnownTypes;

namespace ApiComposition.Api.GrpcClients;

public class RouteClient
{
    private readonly RouteServiceGrpc.RouteServiceGrpcClient _client;

    public RouteClient(RouteServiceGrpc.RouteServiceGrpcClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetRoutesResponseGrpc> GetRoutes(GetRoutesRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.GetRoutesAsync(request, cancellationToken: cancellationToken);
    }
    
    public async Task<CreateRouteResponseGrpc> CreateRoute(CreateRouteRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.CreateRouteAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> UpdateRoute(UpdateRouteRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.UpdateRouteAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> DeleteRoute(DeleteRouteRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.DeleteRouteAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<CreateRouteUnitsResponseGrpc> CreateRouteUnits(CreateRouteUnitsRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.CreateRouteUnitsAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> UpdateRouteUnits(UpdateRouteUnitsRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.UpdateRouteUnitsAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> DeleteRouteUnits(DeleteRouteUnitsRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.DeleteRouteUnitsAsync(request, cancellationToken: cancellationToken);
    }
}