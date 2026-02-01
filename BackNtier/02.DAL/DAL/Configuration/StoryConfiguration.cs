using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configuration
{
    public class StoryConfiguration : IEntityTypeConfiguration<Story>
    {
        public void Configure(EntityTypeBuilder<Story> builder)
        {
            builder.ToTable("Stories");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.Caption)
                .HasMaxLength(500);

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.ExpiresAt)
                .IsRequired();

            // Relationships
            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Views)
                .WithOne(v => v.Story)
                .HasForeignKey(v => v.StoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(s => s.UserId)
                .HasDatabaseName("IX_Stories_UserId");

            builder.HasIndex(s => s.ExpiresAt)
                .HasDatabaseName("IX_Stories_ExpiresAt");

            builder.HasIndex(s => s.CreatedAt)
                .HasDatabaseName("IX_Stories_CreatedAt");
        }
    }
}
