namespace Tariff.Models;

public class TariffUnit
{
    public Guid Id { get; set; }
    public Guid TariffId { get; set; }
    public Guid LocationId { get; set; }
    public Guid NextLocationId { get; set; }
    public double WeightScaleCoefficient { get; set; }
    public int Distance { get; set; }
    public Tariff? Tariff { get; set; }
}