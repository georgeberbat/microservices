using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileDomain;

namespace Profile.Dal.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.CreatedUtc).HasColumnName("created_utc");
        builder.Property(u => u.DeletedUtc).HasColumnName("deleted_utc");
        builder.Property(u => u.UpdatedUtc).HasColumnName("updated_utc");
        builder.Property(u => u.Phone).HasColumnName("phone").IsRequired();
        builder.Property(u => u.Email).HasColumnName("email");
        builder.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
        builder.Property(u => u.Password).HasColumnName("password").IsRequired();
        builder.Property(u => u.FirstName).HasColumnName("first_name");
        builder.Property(u => u.MiddleName).HasColumnName("middle_name");
        builder.Property(u => u.LastName).HasColumnName("last_name");

        builder.HasIndex(u => u.CreatedUtc);
        builder.HasIndex(u => u.UpdatedUtc);
        builder.HasIndex(u => u.DeletedUtc);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Phone).IsUnique();
    }
}