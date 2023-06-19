using Dex.Ef.Contracts.Entities;

namespace Tariff.Models;

public class RouteUnit : ICreatedUtc, IUpdatedUtc
{
    public Guid Id { get; set; }
    public Guid RouteId { get; set; }
    public Guid TariffId { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public Route? Route { get; set; }
    public Tariff? Tariff { get; set; }
}