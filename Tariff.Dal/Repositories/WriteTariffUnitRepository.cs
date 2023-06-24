using Shared.Dal;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class WriteTariffUnitRepository :
        GenericWriteRepository<Models.TariffUnit, Guid, IReadTariffUnitRepository>,
        IWriteTariffUnitRepository
    {
        public WriteTariffUnitRepository(IWriteDbContext writeDbContext)
            : base(writeDbContext, new ReadTariffUnitRepository(writeDbContext))
        {
        }
    }
}