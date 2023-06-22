using Shared.Dal;

namespace Location.Dal.Domain
{
    public interface IWriteLocationRepository : IWriteRepository<Location.Models.Location, Guid, IReadLocationRepository>
    {
    }
}