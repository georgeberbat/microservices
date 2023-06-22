using Location.Dal.Domain;
using Microsoft.EntityFrameworkCore;
using Shared.Dal;

namespace Location.Dal.Repositories
{
    internal class ReadLocationRepository : GenericReadRepository<Models.Location, Guid>, IReadLocationRepository
    {
        public ReadLocationRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }

        public async Task<IEnumerable<Models.Location>> SearchBySubstring(string substring, Func<Models.Location, string> propertyNameGetter, CancellationToken token)
        {
            return BaseQuery.Where(entity =>
                EF.Functions.ILike(
                    EF.Property<string>(entity, propertyNameGetter(default!)),
                    $"%{substring}%"
                )
            );
        }
    }
}