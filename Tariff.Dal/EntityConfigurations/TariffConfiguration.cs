using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tariff.Dal.EntityConfigurations;

public class TariffConfiguration : IEntityTypeConfiguration<Models.Tariff>
{
    public void Configure(EntityTypeBuilder<Models.Tariff> builder)
    {
        builder.ToTable("tariff");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(r => r.UserId).HasColumnName("user_id");
        builder.Property(t => t.Name).HasColumnName("name").IsRequired();
        builder.Property(t => t.CreatedUtc).HasColumnName("created_utc").IsRequired();
        builder.Property(t => t.UpdatedUtc).HasColumnName("updated_utc").IsRequired();
        builder.Property(t => t.DeletedUtc).HasColumnName("deleted_utc");

        builder.HasMany(t => t.TariffUnits)
            .WithOne(tu => tu.Tariff)
            .HasForeignKey(tu => tu.TariffId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TariffUnit_Tariff");

        builder.HasMany(t => t.TariffPermissions)
            .WithOne(tu => tu.Tariff)
            .HasForeignKey(tu => tu.TariffId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TariffPermission_Tariff");
    }
}