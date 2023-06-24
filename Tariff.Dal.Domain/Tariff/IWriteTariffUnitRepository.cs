using Shared.Dal;

namespace Tariff.Dal.Domain.Tariff
{
    public interface IWriteTariffUnitRepository : IWriteRepository<Models.TariffUnit, Guid, IReadTariffUnitRepository>
    {
    }
}