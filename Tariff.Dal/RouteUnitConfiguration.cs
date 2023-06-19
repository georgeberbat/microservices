using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tariff.Models;

namespace Tariff.Dal;


public class RouteUnitConfiguration : IEntityTypeConfiguration<RouteUnit>
{
    public void Configure(EntityTypeBuilder<RouteUnit> builder)
    {
        builder.ToTable("RouteUnit");
        builder.HasKey(ru => ru.Id);

        builder.Property(ru => ru.Id).HasColumnName("id");
        builder.Property(ru => ru.RouteId).HasColumnName("route_id").IsRequired();
        builder.Property(ru => ru.TariffId).HasColumnName("tariff_id").IsRequired();
        builder.Property(ru => ru.CreatedUtc).HasColumnName("created_utc").IsRequired();
        builder.Property(ru => ru.UpdatedUtc).HasColumnName("updated_utc").IsRequired();

        builder.HasOne(ru => ru.Route)
            .WithMany()
            .HasForeignKey(ru => ru.RouteId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RouteUnit_Route");

        builder.HasOne(ru => ru.Tariff)
            .WithMany()
            .HasForeignKey(ru => ru.TariffId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RouteUnit_Tariff");
    }
}