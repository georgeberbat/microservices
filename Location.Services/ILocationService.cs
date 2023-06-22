namespace Location.Services;

public interface ILocationService
{
    Task<IEnumerable<Models.Location>> Get(CancellationToken cancellationToken);
    Task<IEnumerable<Models.Location>> SearchByName(string substring, CancellationToken cancellationToken);
    Task<IEnumerable<Models.Location>> SearchByAddress(string substring, CancellationToken cancellationToken);

    Task<IEnumerable<Models.Location>> SearchByArea(double minLatitude, double maxLatitude, double minLongitude,
        double maxLongitude, CancellationToken cancellationToken);

    Task Create(Models.Location location, CancellationToken cancellationToken);
    Task Update(Models.Location location, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}