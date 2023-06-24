using Shared.Dal;

namespace Tariff.Dal.Domain.Tariff
{
    public interface IReadRouteRepository : IReadRepository<Models.Route, Guid>
    {
    }
}