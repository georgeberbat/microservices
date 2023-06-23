using Dex.Ef.Contracts.Entities;

namespace Tariff.Models;

public class UserTariffPermissions : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid TariffId { get; set; }
    public Guid UserId { get; set; }
    public PermissionMode Mode { get; set; }
    public Tariff? Tariff { get; set; }
}