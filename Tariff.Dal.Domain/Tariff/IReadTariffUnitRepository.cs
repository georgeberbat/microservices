using Shared.Dal;

namespace Tariff.Dal.Domain.Tariff
{
    public interface IReadTariffUnitRepository : IReadRepository<Models.TariffUnit, Guid>
    {
    }
}