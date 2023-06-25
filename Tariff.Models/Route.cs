using Dex.Ef.Contracts.Entities;

namespace Tariff.Models;

public class Route : ICreatedUtc, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedUtc { get; set; }
    public virtual ICollection<RouteUnit>? RouteUnits { get; set; }
}