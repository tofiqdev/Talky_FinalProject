using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class MovieRoomParticipantConfiguration : IEntityTypeConfiguration<MovieRoomParticipant>
    {
        public void Configure(EntityTypeBuilder<MovieRoomParticipant> builder)
        {
            builder.ToTable("MovieRoomParticipants");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

            builder.Property(x => x.MovieRoomId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.JoinedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(x => new { x.MovieRoomId, x.UserId, x.Deleted })
                .IsUnique()
                .HasDatabaseName("idx_MovieRoomParticipant_Unique");

            builder.HasIndex(x => x.UserId);

            // Relationships
            builder.HasOne(x => x.MovieRoom)
                .WithMany(x => x.Participants)
                .HasForeignKey(x => x.MovieRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
