using Shared.Dal;

namespace Tariff.Dal.Domain
{
    public interface IWriteTariffRepository : IWriteRepository<Models.Tariff, Guid, IReadTariffRepository>
    {
    }
}