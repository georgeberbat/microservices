using Shared.Dal;

namespace Location.Dal.Domain
{
    public interface IReadLocationRepository : IReadRepository<Models.Location, Guid>
    {
        Task CheckExistence(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}