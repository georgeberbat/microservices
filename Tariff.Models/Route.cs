using Dex.Ef.Contracts.Entities;

namespace Tariff.Models;

public class Route : ICreatedUtc
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedUtc { get; set; }
    public IEnumerable<RouteUnit>? RouteUnits { get; set; }
    public IEnumerable<UserRoutePermissions>? RoutePermissions { get; set; }
}