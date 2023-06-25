using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tariff.Models;
using Tariff.Services;

namespace Tariff.Api;

public class TariffGrpcService : TariffServiceGrpc.TariffServiceGrpcBase
{
    private readonly ITariffService _tariffService;
    private readonly IMapper _mapper;

    public TariffGrpcService(ITariffService tariffService, IMapper mapper)
    {
        _tariffService = tariffService ?? throw new ArgumentNullException(nameof(tariffService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override async Task<GetTariffsResponseGrpc> GetTariffs(GetTariffsRequestGrpc request,
        ServerCallContext context)
    {
        var tariffs = await _tariffService.Get(Guid.Parse(request.UserId), context.CancellationToken);
        return new GetTariffsResponseGrpc
        {
            Tariffs = { tariffs.Select(x => _mapper.Map<TariffGrpc>(x)) }
        };
    }

    public override async Task<CreateTariffResponseGrpc> CreateTariff(CreateTariffRequestGrpc request,
        ServerCallContext context)
    {
        var id = await _tariffService.Create(Guid.Parse(request.UserId), _mapper.Map<Models.Tariff>(request.Tariff),
            context.CancellationToken);
        return new CreateTariffResponseGrpc
        {
            TariffId = id.ToString()
        };
    }

    public override async Task<Empty> UpdateTariff(UpdateTariffRequestGrpc request, ServerCallContext context)
    {
        await _tariffService.Update(Guid.Parse(request.UserId), _mapper.Map<Models.Tariff>(request.Tariff),
            context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> DeleteTariff(DeleteTariffRequestGrpc request, ServerCallContext context)
    {
        await _tariffService.Delete(Guid.Parse(request.UserId), Guid.Parse(request.TariffId),
            context.CancellationToken);

        return new Empty();
    }

    public override async Task<CreateTariffUnitsResponseGrpc> CreateTariffUnits(CreateTariffUnitsRequestGrpc request,
        ServerCallContext context)
    {
        var mappedEntities = _mapper.Map<TariffUnit[]>(request.TariffUnits.ToArray());
        var units = await _tariffService.CreateEntityUnits(Guid.Parse(request.UserId), mappedEntities,
            context.CancellationToken);

        return new CreateTariffUnitsResponseGrpc
        {
            UnitIds = { units.Select(x => x.ToString()) }
        };
    }

    public override async Task<Empty> UpdateTariffUnits(UpdateTariffUnitsRequestGrpc request, ServerCallContext context)
    {
        var mappedEntities = _mapper.Map<TariffUnit[]>(request.TariffUnits.ToArray());
        await _tariffService.UpdateEntityUnits(Guid.Parse(request.UserId), mappedEntities,
            context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> DeleteTariffUnits(DeleteTariffUnitsRequestGrpc request, ServerCallContext context)
    {
        await _tariffService.DeleteEntityUnits(Guid.Parse(request.UserId), request.UnitIds.Select(Guid.Parse).ToArray(),
            context.CancellationToken);

        return new Empty();
    }
}