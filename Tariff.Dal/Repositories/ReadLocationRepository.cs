using Location.Dal.Domain;
using Shared.Dal;

namespace Tariff.Dal.Repositories
{
    internal class ReadLocationRepository : GenericReadRepository<Location.Models.Location, Guid>,
        IReadLocationRepository
    {
        public ReadLocationRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }

        public Task CheckExistence(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            var exists = BaseQuery.Where(x => ids.Contains(x.Id)).Select(x => x.Id);

            if (ids.Count() < exists.Count())
            {
                throw new InvalidOperationException(); // todo 
            }

            return Task.FromResult(true);
        }
    }
}