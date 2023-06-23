using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tariff.Models;

namespace Tariff.Dal.EntityConfigurations;

public class UserTariffPermissionsConfiguration : IEntityTypeConfiguration<UserTariffPermissions>
{
    public void Configure(EntityTypeBuilder<UserTariffPermissions> builder)
    {
        builder.ToTable("user_tariff_permissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TariffId).HasColumnName("tariff_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Mode).HasColumnName("mode");

        builder.HasOne(ru => ru.Tariff)
            .WithMany()
            .HasForeignKey(ru => ru.TariffId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TariffPermission_Tariff");
    }
}