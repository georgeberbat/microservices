using Shared.Dal;

namespace Tariff.Dal.Domain
{
    public interface IReadTariffRepository : IReadRepository<Models.Tariff, Guid>
    {
        Task<IEnumerable<Models.Tariff>> SearchBySubstring(string substring,
            Func<Models.Tariff, string> propertyNameGetter, CancellationToken token);
    }
}