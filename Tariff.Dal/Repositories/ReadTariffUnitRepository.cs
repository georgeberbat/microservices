using Shared.Dal;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class ReadTariffUnitRepository : GenericReadRepository<Models.TariffUnit, Guid>, IReadTariffUnitRepository
    {
        public ReadTariffUnitRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }
    }
}