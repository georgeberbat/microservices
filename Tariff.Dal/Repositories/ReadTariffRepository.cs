using Microsoft.EntityFrameworkCore;
using Shared.Dal;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class ReadTariffRepository : GenericReadRepository<Models.Tariff, Guid>, IReadTariffRepository
    {
        public ReadTariffRepository(IReadDbContext readDbContext)
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

        protected override IQueryable<Models.Tariff> BaseQuery => base.BaseQuery.Include(x => x.TariffUnits);
    }
}