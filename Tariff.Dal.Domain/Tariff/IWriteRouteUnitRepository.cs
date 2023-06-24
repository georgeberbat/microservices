using Shared.Dal;

namespace Tariff.Dal.Domain.Tariff
{
    public interface IWriteRouteUnitRepository : IWriteRepository<Models.RouteUnit, Guid, IReadRouteUnitRepository>
    {
    }
}