using Google.Protobuf.WellKnownTypes;

namespace ApiComposition.Api.GrpcClients;

public class TariffClient
{
    private readonly TariffServiceGrpc.TariffServiceGrpcClient _client;

    public TariffClient(TariffServiceGrpc.TariffServiceGrpcClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetTariffsResponseGrpc> GetTariffs(GetTariffsRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.GetTariffsAsync(request, cancellationToken: cancellationToken);
    }
    
    public async Task<CreateTariffResponseGrpc> CreateTariff(CreateTariffRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.CreateTariffAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> UpdateTariff(UpdateTariffRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.UpdateTariffAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> DeleteTariff(DeleteTariffRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.DeleteTariffAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<CreateTariffUnitsResponseGrpc> CreateTariffUnits(CreateTariffUnitsRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.CreateTariffUnitsAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> UpdateTariffUnits(UpdateTariffUnitsRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.UpdateTariffUnitsAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<Empty> DeleteTariffUnits(DeleteTariffUnitsRequestGrpc request,
        CancellationToken cancellationToken)
    {
        return await _client.DeleteTariffUnitsAsync(request, cancellationToken: cancellationToken);
    }
}