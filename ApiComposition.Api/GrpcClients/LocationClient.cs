using Google.Protobuf.WellKnownTypes;

namespace ApiComposition.Api.GrpcClients;

public class LocationClient
{
    private readonly LocationGrpc.LocationGrpcClient _client;

    public LocationClient(LocationGrpc.LocationGrpcClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetLocationsGrpcResponse> GetLocations(CancellationToken cancellationToken)
    {
        return await _client.GetLocationsAsync(new Empty(), cancellationToken: cancellationToken);
    }

    public async Task<GetLocationsGrpcResponse> SearchByName(string substring, CancellationToken cancellationToken)
    {
        return await _client.SearchByNameAsync(new SearchByNameGrpcRequest { Substring = substring },
            cancellationToken: cancellationToken);
    }

    public async Task<GetLocationsGrpcResponse> SearchByAddress(string substring, CancellationToken cancellationToken)
    {
        return await _client.SearchByAddressAsync(new SearchByAddressGrpcRequest { Substring = substring },
            cancellationToken: cancellationToken);
    }

    public async Task<GetLocationsGrpcResponse> SearchByArea(SearchByAreaGrpcRequest request,
        CancellationToken cancellationToken)
    {
        return await _client.SearchByAreaAsync(request, cancellationToken: cancellationToken);
    }

    public async Task CreateLocation(CreateLocationGrpcRequest request, CancellationToken cancellationToken)
    {
        await _client.CreateLocationAsync(request, cancellationToken: cancellationToken);
    }

    public async Task UpdateLocation(UpdateLocationGrpcRequest request, CancellationToken cancellationToken)
    {
        await _client.UpdateLocationAsync(request, cancellationToken: cancellationToken);
    }

    public async Task DeleteLocation(DeleteLocationGrpcRequest request, CancellationToken cancellationToken)
    {
        await _client.DeleteLocationAsync(request, cancellationToken: cancellationToken);
    }
}