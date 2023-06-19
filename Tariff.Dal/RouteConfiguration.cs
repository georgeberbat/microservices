using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tariff.Models;

namespace Tariff.Dal;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Route");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("id");
        builder.Property(r => r.UserId).HasColumnName("user_id");
        builder.Property(r => r.Name).HasColumnName("name").IsRequired();
        builder.Property(r => r.Description).HasColumnName("description");
        builder.Property(r => r.CreatedUtc).HasColumnName("created_utc").IsRequired();

        builder.HasMany(r => r.RouteUnits)
            .WithOne(ru => ru.Route)
            .HasForeignKey(ru => ru.RouteId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RouteUnit_Route");
    }
}