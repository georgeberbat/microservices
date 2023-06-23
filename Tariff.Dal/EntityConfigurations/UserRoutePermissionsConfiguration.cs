using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tariff.Models;

namespace Tariff.Dal.EntityConfigurations;

public class UserRoutePermissionsConfiguration : IEntityTypeConfiguration<UserRoutePermissions>
{
    public void Configure(EntityTypeBuilder<UserRoutePermissions> builder)
    {
        builder.ToTable("user_route_permissions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.RouteId, x.UserId }).IsUnique();

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.RouteId).HasColumnName("route_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Mode).HasColumnName("mode");

        builder.HasOne(ru => ru.Route)
            .WithMany()
            .HasForeignKey(ru => ru.RouteId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RoutePermission_Route");
    }
}