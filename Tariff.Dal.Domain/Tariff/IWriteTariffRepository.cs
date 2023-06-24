using Shared.Dal;

namespace Tariff.Dal.Domain.Tariff
{
    public interface IWriteTariffRepository : IWriteRepository<Models.Tariff, Guid, IReadTariffRepository>
    {
    }
}