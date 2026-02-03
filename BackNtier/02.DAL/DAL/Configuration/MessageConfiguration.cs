using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            // Tablo adı
            builder.ToTable("Messages");

            // Primary Key
            builder.HasKey(x => x.Id);

            // BaseEntity'den gelen property'ler
            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

           

            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            // Message sınıfındaki property'ler
            builder.Property(x => x.SenderId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.ReceiverId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)")
                .IsUnicode(true);

            builder.Property(x => x.IsRead)
                .HasColumnType("bit")
                .HasDefaultValue(false);

            builder.Property(x => x.SentAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.ReadAt)
                .HasColumnType("datetime")
                .IsRequired(false);

            // Index'ler
            builder.HasIndex(x => x.Deleted)
                .HasDatabaseName("idx_Deleted");

            builder.HasIndex(x => x.SenderId)
                .HasDatabaseName("idx_SenderId");

            builder.HasIndex(x => x.ReceiverId)
                .HasDatabaseName("idx_ReceiverId");

            builder.HasIndex(x => x.SentAt)
                .HasDatabaseName("idx_SentAt");

            builder.HasIndex(x => x.IsRead)
                .HasDatabaseName("idx_IsRead");

            // Foreign Key ilişkileri
            builder.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Receiver)
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}