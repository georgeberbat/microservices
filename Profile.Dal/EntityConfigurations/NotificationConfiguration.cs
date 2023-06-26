using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileDomain;

namespace Profile.Dal.EntityConfigurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notification");

        builder.HasKey(n => n.Id)
            .HasName("id");

        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(n => n.CreatedUtc)
            .HasColumnName("created_utc");

        builder.Property(n => n.Text)
            .HasColumnName("text")
            .IsRequired();

        builder.Property(n => n.Title)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(n => n.Viewed)
            .HasColumnName("viewed");

        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Notification_User");
        
        builder.HasIndex(n => n.CreatedUtc).HasDatabaseName("IX_Notification_CreatedUtc_Desc");
    }
}