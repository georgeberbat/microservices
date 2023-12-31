﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tariff.Models;

namespace Tariff.Dal.EntityConfigurations;

public class TariffUnitConfiguration : IEntityTypeConfiguration<TariffUnit>
{
    public void Configure(EntityTypeBuilder<TariffUnit> builder)
    {
        builder.ToTable("tariff_unit");
        builder.HasKey(tu => tu.Id);

        builder.Property(tu => tu.Id).HasColumnName("id");
        builder.Property(tu => tu.TariffId).HasColumnName("tariff_id").IsRequired();
        builder.Property(tu => tu.LocationId).HasColumnName("location_id").IsRequired();
        builder.Property(tu => tu.NextLocationId).HasColumnName("next_location_id").IsRequired();
        builder.Property(tu => tu.WeightScaleCoefficient).HasColumnName("weight_scale_coefficient").IsRequired();
        builder.Property(tu => tu.Distance).HasColumnName("distance").IsRequired();
        builder.Property(l => l.CreatedUtc).HasColumnName("created_utc");
        builder.Property(l => l.UpdatedUtc).HasColumnName("updated_utc");

        builder.HasOne(tu => tu.Tariff)
            .WithMany(t => t.TariffUnits)
            .HasForeignKey(tu => tu.TariffId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TariffUnit_Tariff");
    }
}