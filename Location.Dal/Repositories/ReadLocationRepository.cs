using Location.Dal.Domain;
using Shared.Dal;

namespace Location.Dal.Repositories
{
    internal class ReadLocationRepository : GenericReadRepository<Models.Location, Guid>, IReadLocationRepository
    {
        public ReadLocationRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }

        public Task CheckExistence(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}