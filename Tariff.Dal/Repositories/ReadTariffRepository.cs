using Microsoft.EntityFrameworkCore;
using Shared.Dal;
using Tariff.Dal.Domain;

namespace Tariff.Dal.Repositories
{
    internal class ReadTariffRepository : GenericReadRepository<Models.Tariff, Guid>, IReadTariffRepository
    {
        public ReadTariffRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }

        public async Task<IEnumerable<Models.Tariff>> SearchBySubstring(string substring, Func<Models.Tariff, string> propertyNameGetter, CancellationToken token)
        {
            var value = propertyNameGetter(default!);
            return BaseQuery.Where(entity =>
                EF.Functions.ILike(
                    EF.Property<string>(entity, value),
                    $"%{substring}%"
                )
            );
        }
    }
}