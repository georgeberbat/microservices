using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Route.Dal;

public class RouteConfiguration : IEntityTypeConfiguration<Models.Route>
{
    public void Configure(EntityTypeBuilder<Models.Route> builder)
    {
        builder.ToTable("route");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("id");
        builder.Property(r => r.Name).HasColumnName("name").IsRequired();
        builder.Property(r => r.Description).HasColumnName("description");

        builder.HasMany(r => r.RouteLocations)
            .WithOne(rl => rl.Route)
            .HasForeignKey(rl => rl.RouteId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RouteLocation_Route");
    }
}