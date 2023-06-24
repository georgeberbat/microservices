using Shared.Dal;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class WriteRouteRepository : GenericWriteRepository<Models.Route, Guid, IReadRouteRepository>,
        IWriteRouteRepository
    {
        public WriteRouteRepository(IWriteDbContext writeDbContext)
            : base(writeDbContext, new ReadRouteRepository(writeDbContext))
        {
        }
    }
}