using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class GroupMessageConfiguration : IEntityTypeConfiguration<GroupMessage>
    {
        public void Configure(EntityTypeBuilder<GroupMessage> builder)
        {
            // Tablo adı
            builder.ToTable("GroupMessages");

            // Primary Key (BaseEntity'den geliyor)
            builder.HasKey(x => x.Id);

            // BaseEntity'den gelen property'ler
            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

            // Eğer BaseEntity'de Name property'si varsa
            // Name property removed


            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            // GroupMessage sınıfındaki property'ler
            builder.Property(x => x.GroupId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.SenderId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnType("nvarchar(max)")
                .IsRequired()
                .IsUnicode(true);

            builder.Property(x => x.IsSystemMessage)
                .HasColumnType("bit")
                .HasDefaultValue(false);

            builder.Property(x => x.SentAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            // Index'ler - ÇOK ÖNEMLİ (performans için)

            // Grup mesajlarını tarihe göre sıralamak için
            builder.HasIndex(x => new { x.GroupId, x.SentAt })
                .HasDatabaseName("idx_GroupMessage_Group_SentAt");

            // Gönderene göre arama için
            builder.HasIndex(x => x.SenderId)
                .HasDatabaseName("idx_GroupMessage_SenderId");

            // Sistem mesajlarını filtrelemek için
            builder.HasIndex(x => new { x.GroupId, x.IsSystemMessage })
                .HasDatabaseName("idx_GroupMessage_Group_IsSystem");

            // Silinmemiş mesajları hızlı getirmek için
            builder.HasIndex(x => new { x.GroupId, x.Deleted, x.SentAt })
                .HasDatabaseName("idx_GroupMessage_Group_Deleted_SentAt");

            // Foreign Key ilişkileri

            // Group ile ilişki
            builder.HasOne(x => x.Group)
                .WithMany(g => g.Messages) // Group entity'sindeki Messages koleksiyonu
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // Grup silinirse mesajlar da silinsin

            // Sender (User) ile ilişki
            builder.HasOne(x => x.Sender)
                .WithMany() // User entity'sinde GroupMessage koleksiyonu yoksa boş bırakın
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silinirse mesajlar kalsın

            // Check constraint'ler (opsiyonel)
            builder.HasCheckConstraint("CK_GroupMessage_SentAt",
                "[SentAt] <= GETUTCDATE()");

            builder.HasCheckConstraint("CK_GroupMessage_Content",
                "LEN([Content]) > 0");
        }
    }
}   