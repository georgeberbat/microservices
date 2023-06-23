using Dex.Ef.Contracts.Entities;

namespace Tariff.Models;

public class UserRoutePermissions : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid RouteId { get; set; }
    public Guid UserId { get; set; }
    public PermissionMode Mode { get; set; }
    public Route? Route { get; set; }
}