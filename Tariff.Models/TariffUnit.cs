﻿using Dex.Ef.Contracts.Entities;

namespace Tariff.Models;

public class TariffUnit : IEntity<Guid>, ICreatedUtc, IUpdatedUtc
{
    public Guid Id { get; set; }
    public Guid TariffId { get; set; }
    public Guid LocationId { get; set; }
    public Guid NextLocationId { get; set; }
    public double WeightScaleCoefficient { get; set; }
    public int Distance { get; set; }
    public virtual Tariff? Tariff { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}