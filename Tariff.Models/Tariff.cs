using Dex.Ef.Contracts.Entities;

namespace Tariff.Models;

public class Tariff : ICreatedUtc, IUpdatedUtc, IDeletable, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public DateTime? DeletedUtc { get; set; }
    public IEnumerable<TariffUnit>? TariffUnits { get; set; }
    public IEnumerable<UserTariffPermissions>? TariffPermissions { get; set; }
}