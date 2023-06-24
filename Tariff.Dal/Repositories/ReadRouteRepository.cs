using Shared.Dal;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class ReadRouteRepository : GenericReadRepository<Models.Route, Guid>, IReadRouteRepository
    {
        public ReadRouteRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }
    }
}