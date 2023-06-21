namespace Location.Services;

public interface ILocationService
{
    IEnumerable<Models.Location> Get();
    IEnumerable<Models.Location> SearchByName(string substring);
    IEnumerable<Models.Location> SearchByAddress(string substring);
    IEnumerable<Models.Location> SearchByArea(double minLatitude, double maxLatitude, double minLongitude, double maxLongitude);
    
    void Create(Models.Location location);
    void Update(Models.Location location);
    void Delete(Guid id);
}