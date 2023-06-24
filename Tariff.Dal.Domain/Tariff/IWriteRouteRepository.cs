using Shared.Dal;

namespace Tariff.Dal.Domain.Tariff
{
    public interface IWriteRouteRepository : IWriteRepository<Models.Route, Guid, IReadRouteRepository>
    {
    }
}