using Microsoft.EntityFrameworkCore;
using Shared.Dal;
using Tariff.Dal.Domain.Tariff;
using Tariff.Models;

namespace Tariff.Dal.Repositories
{
    internal class ReadRouteRepository : GenericReadRepository<Route, Guid>, IReadRouteRepository
    {
        public ReadRouteRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<Route> BaseQuery => base.BaseQuery.Include(x => x.RouteUnits);
    }
}