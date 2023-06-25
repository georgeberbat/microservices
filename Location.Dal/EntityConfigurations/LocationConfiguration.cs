using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Dal.EntityConfigurations;

public class LocationConfiguration : IEntityTypeConfiguration<Models.Location>
{
    public void Configure(EntityTypeBuilder<Models.Location> builder)
    {
        builder.ToTable("location");
        builder.HasKey(l => l.Id).HasName("id");
        builder.Property(l => l.Name).HasColumnName("name").IsRequired();
        builder.Property(l => l.Address).HasColumnName("address").IsRequired();
        builder.Property(l => l.Latitude).HasColumnName("latitude");
        builder.Property(l => l.Longitude).HasColumnName("longitude");
        builder.Property(l => l.DeletedUtc).HasColumnName("deleted_utc");
        builder.Property(l => l.CreatedUtc).HasColumnName("created_utc");
        builder.Property(l => l.UpdatedUtc).HasColumnName("updated_utc");

        builder.HasIndex(l => l.CreatedUtc);
        builder.HasIndex(l => l.UpdatedUtc);
    }
}