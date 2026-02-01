using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            // Tablo adı
            builder.ToTable("Groups");

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

            // Group sınıfındaki property'ler
            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false)
                .IsUnicode(true);

            builder.Property(x => x.Avatar)
                .HasColumnType("nvarchar(500)")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.CreatedById)
                .HasColumnType("int")
                .IsRequired();

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
                .HasDatabaseName("idx_Group_Name_Deleted");

            builder.HasIndex(x => x.CreatedById)
                .HasDatabaseName("idx_Group_CreatedById");

            builder.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("idx_Group_CreatedAt");

            builder.HasIndex(x => x.UpdatedAt)
                .HasDatabaseName("idx_Group_UpdatedAt");

            // Foreign Key ilişkileri

            // CreatedBy (Grup oluşturan kullanıcı)
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Members (Grup üyeleri)
            builder.HasMany(x => x.Members)
                .WithOne(x => x.Group)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Messages (Grup mesajları)
            builder.HasMany(x => x.Messages)
                .WithOne(x => x.Group)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraint (opsiyonel)
            builder.HasCheckConstraint("CK_Group_UpdatedAt",
                "[UpdatedAt] >= [CreatedAt]");
        }
    }
}