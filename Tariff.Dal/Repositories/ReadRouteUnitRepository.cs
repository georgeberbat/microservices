using Shared.Dal;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class ReadRouteUnitRepository : GenericReadRepository<Models.RouteUnit, Guid>, IReadRouteUnitRepository
    {
        public ReadRouteUnitRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }
    }
}