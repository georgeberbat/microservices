using Location.Dal.Domain;
using Shared.Dal;
using Shared.Exceptions;

namespace Location.Dal.Repositories
{
    internal class ReadLocationRepository : GenericReadRepository<Models.Location, Guid>, IReadLocationRepository
    {
        public ReadLocationRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }
    }
}