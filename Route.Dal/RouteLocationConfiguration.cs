using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Route.Models;

namespace Route.Dal;

public class RouteLocationConfiguration : IEntityTypeConfiguration<RouteLocation>
{
    public void Configure(EntityTypeBuilder<RouteLocation> builder)
    {
        builder.ToTable("route_location");
        builder.HasKey(rl => rl.Id);

        builder.Property(rl => rl.Id).HasColumnName("id");
        builder.Property(rl => rl.RouteId).HasColumnName("route_id").IsRequired();
        builder.Property(rl => rl.LocationId).HasColumnName("location_id").IsRequired();
        builder.Property(rl => rl.Weight).HasColumnName("distance").IsRequired();

        builder.HasOne(rl => rl.Route)
            .WithMany(r => r.RouteLocations)
            .HasForeignKey(rl => rl.RouteId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RouteLocation_Route");

        builder.HasOne(rl => rl.Location)
            .WithMany()
            .HasForeignKey(rl => rl.LocationId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RouteLocation_Location");
    }
}