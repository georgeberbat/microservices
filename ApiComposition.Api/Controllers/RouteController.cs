using ApiComposition.Api.GrpcClients;
using ApiComposition.Api.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;
using Shared.Interfaces;

namespace ApiComposition.Api.Controllers;

public class RouteController : BaseController
{
    private readonly RouteClient _routeClient;
    private readonly IUserIdHttpContextService _userId;

    public RouteController(RouteClient routeClient, IUserIdHttpContextService userId)
    {
        _routeClient = routeClient ?? throw new ArgumentNullException(nameof(routeClient));
        _userId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    [HttpGet]
    public async Task<IActionResult> GetRoutes(CancellationToken cancellationToken)
    {
        var response =
            await _routeClient.GetRoutes(new GetRoutesRequestGrpc { UserId = _userId.UserId.ToString() },
                cancellationToken);

        return Ok(response.Routes);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoute(RouteGrpc route,
        CancellationToken cancellationToken)
    {
        var response =
            await _routeClient.CreateRoute(new CreateRouteRequestGrpc
                {
                    Route = route,
                    UserId = _userId.UserId.ToString()
                },
                cancellationToken);

        return Ok(response.RouteId);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRoute(RouteGrpc route,
        CancellationToken cancellationToken)
    {
        await _routeClient.UpdateRoute(new UpdateRouteRequestGrpc
            {
                Route = route,
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRoute(Guid routeId,
        CancellationToken cancellationToken)
    {
        await _routeClient.DeleteRoute(new DeleteRouteRequestGrpc
            {
                RouteId = routeId.ToString(),
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRouteUnits([FromBody] ArrayRequest<RouteUnitGrpc> routeUnits,
        CancellationToken cancellationToken)
    {
        var response =
            await _routeClient.CreateRouteUnits(new CreateRouteUnitsRequestGrpc
                {
                    RouteUnits = { routeUnits.Items },
                    UserId = _userId.UserId.ToString()
                },
                cancellationToken);

        return Ok(response.UnitIds.ToArray());
    }


    [HttpPut]
    public async Task<IActionResult> UpdateRouteUnits(ArrayRequest<RouteUnitGrpc> routeUnits,
        CancellationToken cancellationToken)
    {
        await _routeClient.UpdateRouteUnits(new UpdateRouteUnitsRequestGrpc
            {
                RouteUnits = { routeUnits.Items },
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRouteUnits(IEnumerable<Guid> routeUnitIds,
        CancellationToken cancellationToken)
    {
        await _routeClient.DeleteRouteUnits(new DeleteRouteUnitsRequestGrpc
            {
                UnitIds = { routeUnitIds.Select(x => x.ToString()).ToArray() },
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }
}