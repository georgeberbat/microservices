using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tariff.Models;
using Tariff.Services;

namespace Tariff.Api;

public class RouteGrpcService : RouteServiceGrpc.RouteServiceGrpcBase
{
    private readonly IRouteService _routeService;
    private readonly IMapper _mapper;

    public RouteGrpcService(IRouteService routeService, IMapper mapper)
    {
        _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override async Task<GetRoutesResponseGrpc> GetRoutes(GetRoutesRequestGrpc request,
        ServerCallContext context)
    {
        var routes = await _routeService.Get(Guid.Parse(request.UserId), context.CancellationToken);
        return new GetRoutesResponseGrpc
        {
            Routes = { routes.Select(x => _mapper.Map<RouteGrpc>(x)) }
        };
    }

    public override async Task<CreateRouteResponseGrpc> CreateRoute(CreateRouteRequestGrpc request,
        ServerCallContext context)
    {
        var id = await _routeService.Create(Guid.Parse(request.UserId), _mapper.Map<Route>(request),
            context.CancellationToken);
        return new CreateRouteResponseGrpc
        {
            RouteId = id.ToString()
        };
    }

    public override async Task<Empty> UpdateRoute(UpdateRouteRequestGrpc request, ServerCallContext context)
    {
        await _routeService.Update(Guid.Parse(request.UserId), _mapper.Map<Route>(request),
            context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> DeleteRoute(DeleteRouteRequestGrpc request, ServerCallContext context)
    {
        await _routeService.Delete(Guid.Parse(request.UserId), Guid.Parse(request.RouteId),
            context.CancellationToken);

        return new Empty();
    }

    public override async Task<CreateRouteUnitsResponseGrpc> CreateRouteUnits(CreateRouteUnitsRequestGrpc request,
        ServerCallContext context)
    {
        var mappedEntities = _mapper.Map<RouteUnit[]>(request.RouteUnits.ToArray());
        var units = await _routeService.CreateEntityUnits(Guid.Parse(request.UserId), mappedEntities,
            context.CancellationToken);

        return new CreateRouteUnitsResponseGrpc
        {
            UnitIds = { units.Select(x => x.ToString()) }
        };
    }

    public override async Task<Empty> UpdateRouteUnits(UpdateRouteUnitsRequestGrpc request, ServerCallContext context)
    {
        var mappedEntities = _mapper.Map<RouteUnit[]>(request.RouteUnits.ToArray());
        await _routeService.UpdateEntityUnits(Guid.Parse(request.UserId), mappedEntities,
            context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> DeleteRouteUnits(DeleteRouteUnitsRequestGrpc request, ServerCallContext context)
    {
        await _routeService.DeleteEntityUnits(Guid.Parse(request.UserId), request.UnitIds.Select(Guid.Parse).ToArray(),
            context.CancellationToken);

        return new Empty();
    }
}