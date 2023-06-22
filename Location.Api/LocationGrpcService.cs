using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Location.Services;

namespace Location.Api;

public class LocationGrpcService : LocationGrpc.LocationGrpcBase
{
    private readonly ILocationService _locationService;
    private readonly IMapper _mapper;

    public LocationGrpcService(ILocationService locationService, IMapper mapper)
    {
        _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override async Task<GetLocationsGrpcResponse> GetLocations(Empty request, ServerCallContext context)
    {
        var locations = await _locationService.Get(context.CancellationToken);
        return new GetLocationsGrpcResponse
        {
            Locations = { locations.Select(x => _mapper.Map<LocationGrpcModel>(x)) }
        };
    }

    public override async Task<GetLocationsGrpcResponse> SearchByName(SearchByNameGrpcRequest request,
        ServerCallContext context)
    {
        var locations = (await _locationService.SearchByName(request.Substring, context.CancellationToken)).ToArray();
        return new GetLocationsGrpcResponse
        {
            Locations = { locations.Select(x => _mapper.Map<LocationGrpcModel>(x)) }
        };
    }

    public override async Task<GetLocationsGrpcResponse> SearchByAddress(SearchByAddressGrpcRequest request,
        ServerCallContext context)
    {
        var locations = (await _locationService.SearchByAddress(request.Substring, context.CancellationToken))
            .ToArray();
        return new GetLocationsGrpcResponse
        {
            Locations = { locations.Select(x => _mapper.Map<LocationGrpcModel>(x)) }
        };
    }

    public override async Task<GetLocationsGrpcResponse> SearchByArea(SearchByAreaGrpcRequest request,
        ServerCallContext context)
    {
        var locations = await _locationService.SearchByArea(
            request.MinLatitude, request.MaxLatitude, request.MinLongitude, request.MaxLongitude,
            context.CancellationToken);

        return new GetLocationsGrpcResponse
        {
            Locations = { locations.Select(x => _mapper.Map<LocationGrpcModel>(x)) }
        };
    }

    public override async Task<Empty> CreateLocation(CreateLocationGrpcRequest request, ServerCallContext context)
    { 
        await _locationService.Create(_mapper.Map<Models.Location>(request.Location), context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> UpdateLocation(UpdateLocationGrpcRequest request, ServerCallContext context)
    {
        await _locationService.Update(_mapper.Map<Models.Location>(request.Location), context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> DeleteLocation(DeleteLocationGrpcRequest request, ServerCallContext context)
    {
        await _locationService.Delete(Guid.Parse(request.Id), context.CancellationToken);
        return new Empty();
    }
}