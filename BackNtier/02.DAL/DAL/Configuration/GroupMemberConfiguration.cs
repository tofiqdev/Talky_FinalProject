using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            // Tablo adı
            builder.ToTable("GroupMembers");

            // Primary Key (BaseEntity'den geliyor)
            builder.HasKey(x => x.Id);

            // BaseEntity'den gelen property'ler
            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            // GroupMember sınıfındaki property'ler
            builder.Property(x => x.GroupId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.IsAdmin)
                .HasColumnType("bit")
                .HasDefaultValue(false);

            builder.Property(x => x.IsMuted)
                .HasColumnType("bit")
                .HasDefaultValue(false);

            builder.Property(x => x.JoinedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            // Index'ler - ÇOK ÖNEMLİ!

            // Composite unique index: Bir kullanıcı aynı gruba sadece bir kez eklenebilir
            builder.HasIndex(x => new { x.GroupId, x.UserId, x.Deleted })
                .IsUnique()
                .HasDatabaseName("idx_GroupMember_Group_User_Deleted");

            // UserId index'i (performans için)
            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("idx_GroupMember_UserId");

            // GroupId index'i (performans için)
            builder.HasIndex(x => x.GroupId)
                .HasDatabaseName("idx_GroupMember_GroupId");

            // IsAdmin index'i (admin aramaları için)
            builder.HasIndex(x => new { x.GroupId, x.IsAdmin })
                .HasDatabaseName("idx_GroupMember_Group_IsAdmin");

            // Foreign Key ilişkileri

            // Group ile ilişki
            builder.HasOne(x => x.Group)
                .WithMany(g => g.Members) // Group entity'sindeki Members koleksiyonu
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // Grup silinirse üyeler de silinsin

            // User ile ilişki
            builder.HasOne(x => x.User)
                .WithMany() // User entity'sinde GroupMember koleksiyonu yoksa boş bırakın
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silinirse grup üyeliği kalsın

            // Check constraint (opsiyonel)
            builder.HasCheckConstraint("CK_GroupMember_JoinedAt",
                "[JoinedAt] <= GETUTCDATE()");
        }
    }
}