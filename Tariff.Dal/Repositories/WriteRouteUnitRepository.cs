using Shared.Dal;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class WriteRouteUnitRepository :
        GenericWriteRepository<Models.RouteUnit, Guid, IReadRouteUnitRepository>,
        IWriteRouteUnitRepository
    {
        public WriteRouteUnitRepository(IWriteDbContext writeDbContext)
            : base(writeDbContext, new ReadRouteUnitRepository(writeDbContext))
        {
        }
    }
}