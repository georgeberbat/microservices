using Shared.Dal;

namespace Tariff.Dal.Domain.Tariff
{
    public interface IReadTariffRepository : IReadRepository<Models.Tariff, Guid>
    {
        Task CheckExistence(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}