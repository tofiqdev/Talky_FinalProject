using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Tablo adı
            builder.ToTable("Users");

            // Primary Key
            builder.HasKey(x => x.Id);

            // BaseEntity'den gelen property'ler
            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(250);

            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            // AppUser ilişkisi
            builder.Property(x => x.AppUserId)
                .HasColumnType("int")
                .IsRequired(false);

            // User sınıfındaki property'ler
            builder.Property(x => x.Username)
                .IsRequired()
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("nvarchar(255)")
                .HasMaxLength(255);

            builder.Property(x => x.PasswordHash)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false); // AppUser varsa gerekli olmayabilir

            builder.Property(x => x.Avatar)
                .HasColumnType("nvarchar(500)")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.Bio)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false)
                .IsUnicode(true);

            builder.Property(x => x.IsOnline)
                .HasColumnType("bit")
                .HasDefaultValue(false);

            builder.Property(x => x.LastSeen)
                .HasColumnType("datetime")
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            // Index'ler
            builder.HasIndex(x => new { x.Name, x.Deleted })
                .IsUnique()
                .HasDatabaseName("idx_Name_Deleted");

            builder.HasIndex(x => x.Username)
                .IsUnique()
                .HasDatabaseName("idx_Username");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("idx_Email");

            builder.HasIndex(x => x.AppUserId)
                .IsUnique() // One-to-One ilişki için
                .HasDatabaseName("idx_AppUserId");

            builder.HasIndex(x => x.IsOnline)
                .HasDatabaseName("idx_IsOnline");

            builder.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("idx_CreatedAt");

            // AppUser ile One-to-One ilişki
            builder.HasOne(x => x.AppUser)
                .WithOne()
                .HasForeignKey<User>(x => x.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Diğer navigation property'ler
            builder.HasMany(x => x.SentMessages)
                .WithOne(x => x.Sender)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ReceivedMessages)
                .WithOne(x => x.Receiver)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.InitiatedCalls)
                .WithOne(x => x.Caller)
                .HasForeignKey(x => x.CallerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ReceivedCalls)
                .WithOne(x => x.Receiver)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}