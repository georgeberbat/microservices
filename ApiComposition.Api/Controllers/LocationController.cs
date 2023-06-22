using ApiComposition.Api.GrpcClients;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;

namespace ApiComposition.Api.Controllers;

public class LocationController : BaseController
{
    private readonly LocationClient _locationClient;

    public LocationController(LocationClient locationClient)
    {
        _locationClient = locationClient ?? throw new ArgumentNullException(nameof(locationClient));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetLocations(CancellationToken cancellationToken)
    {
        var response = await _locationClient.GetLocations(cancellationToken);
        return Ok(response.Locations);
    }

    [HttpGet]
    public async Task<IActionResult> SearchByName([FromQuery] string substring, CancellationToken cancellationToken)
    {
        var response = await _locationClient.SearchByName(substring, cancellationToken);
        return Ok(response.Locations);
    }

    [HttpGet]
    public async Task<IActionResult> SearchByAddress([FromQuery] string substring, CancellationToken cancellationToken)
    {
        var response = await _locationClient.SearchByAddress(substring, cancellationToken);
        return Ok(response.Locations);
    }

    [HttpPost]
    public async Task<IActionResult> SearchByArea([FromBody] SearchByAreaGrpcRequest request, CancellationToken cancellationToken)
    {
        var response = await _locationClient.SearchByArea(request, cancellationToken);
        return Ok(response.Locations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationGrpcRequest request, CancellationToken cancellationToken)
    {
        await _locationClient.CreateLocation(request, cancellationToken);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationGrpcRequest request, CancellationToken cancellationToken)
    {
        await _locationClient.UpdateLocation(request, cancellationToken);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteLocation([FromBody] DeleteLocationGrpcRequest request, CancellationToken cancellationToken)
    {
        await _locationClient.DeleteLocation(request, cancellationToken);
        return Ok();
    }
}