using Shared.Dal;

namespace Location.Dal.Domain
{
    public interface IReadLocationRepository : IReadRepository<Models.Location, Guid>
    {
    }
}