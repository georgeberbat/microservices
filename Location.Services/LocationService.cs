using Location.Dal.Domain;
using Location.Models.Commands;
using Location.Services.Specifications;
using MassTransit;
using Shared.Dal;
using Shared.Dal.Specifications;

namespace Location.Services;

internal class LocationService : ILocationService
{
    private readonly IWriteLocationRepository _locationRepository;
    private readonly IUnityOfWork _dbContext;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public LocationService(IWriteLocationRepository locationRepository, IUnityOfWork dbContext,
        ISendEndpointProvider sendEndpointProvider)
    {
        _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
    }

    public async Task<IEnumerable<Models.Location>> Get(CancellationToken cancellationToken)
    {
        return await _locationRepository.Read.FilterAsync(new UndeletedSpecification<Models.Location>(),
            cancellationToken);
    }

    public async Task<IEnumerable<Models.Location>> SearchByName(string substring, CancellationToken cancellationToken)
    {
        return await _locationRepository.Read.SearchBySubstring(substring, x => nameof(x.Name),
            cancellationToken);
    }

    public async Task<IEnumerable<Models.Location>> SearchByAddress(string substring,
        CancellationToken cancellationToken)
    {
        return await _locationRepository.Read.SearchBySubstring(substring, x => nameof(x.Address),
            cancellationToken);
    }

    public async Task<IEnumerable<Models.Location>> SearchByArea(double minLatitude, double maxLatitude,
        double minLongitude, double maxLongitude,
        CancellationToken cancellationToken)
    {
        return await _locationRepository.Read.FilterAsync(new InsideAreaSpecification(minLatitude, maxLatitude,
            minLongitude, maxLongitude), cancellationToken);
    }

    public async Task Create(Models.Location location, CancellationToken cancellationToken)
    {
        await _locationRepository.AddAsync(location, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Models.Location location, CancellationToken cancellationToken)
    {
        await _locationRepository.UpdateAsync(location, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.Read.GetByIdAsync(id, cancellationToken);
        await _locationRepository.RemoveAsync(location, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _sendEndpointProvider.Send(new OnLocationRemovedCommand { LocationId = location.Id, Name = location.Name },
            cancellationToken);
    }
}