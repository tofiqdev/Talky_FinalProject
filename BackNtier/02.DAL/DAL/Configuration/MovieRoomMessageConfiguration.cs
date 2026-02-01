using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class MovieRoomMessageConfiguration : IEntityTypeConfiguration<MovieRoomMessage>
    {
        public void Configure(EntityTypeBuilder<MovieRoomMessage> builder)
        {
            builder.ToTable("MovieRoomMessages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

            builder.Property(x => x.MovieRoomId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.SenderId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.SentAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(x => x.MovieRoomId);
            builder.HasIndex(x => x.SenderId);
            builder.HasIndex(x => x.SentAt);

            // Relationships
            builder.HasOne(x => x.MovieRoom)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.MovieRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
