using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class MovieRoomConfiguration : IEntityTypeConfiguration<MovieRoom>
    {
        public void Configure(EntityTypeBuilder<MovieRoom> builder)
        {
            builder.ToTable("MovieRooms");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(250);

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(x => x.YouTubeUrl)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(500);

            builder.Property(x => x.YouTubeVideoId)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder.Property(x => x.CreatedById)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnType("bit")
                .HasDefaultValue(true);

            builder.Property(x => x.CurrentTime)
                .HasColumnType("float")
                .HasDefaultValue(0);

            builder.Property(x => x.IsPlaying)
                .HasColumnType("bit")
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => x.CreatedAt);

            // Relationships
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Participants)
                .WithOne(x => x.MovieRoom)
                .HasForeignKey(x => x.MovieRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Messages)
                .WithOne(x => x.MovieRoom)
                .HasForeignKey(x => x.MovieRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
