using Shared.Dal;

namespace Location.Dal.Domain
{
    public interface IReadLocationRepository : IReadRepository<Models.Location, Guid>
    {
        Task<IEnumerable<Models.Location>> SearchBySubstring(string substring,
            Func<Models.Location, string> propertyNameGetter, CancellationToken token);
    }
}