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
    public class StoryViewConfiguration : IEntityTypeConfiguration<StoryView>
    {
        public void Configure(EntityTypeBuilder<StoryView> builder)
        {
            builder.ToTable("StoryViews");

            builder.HasKey(sv => sv.Id);

            builder.Property(sv => sv.StoryId)
                .IsRequired();

            builder.Property(sv => sv.UserId)
                .IsRequired();

            builder.Property(sv => sv.ViewedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(sv => sv.Story)
                .WithMany(s => s.Views)
                .HasForeignKey(sv => sv.StoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sv => sv.User)
                .WithMany()
                .HasForeignKey(sv => sv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sv => sv.StoryId)
                .HasDatabaseName("IX_StoryViews_StoryId");

            builder.HasIndex(sv => sv.UserId)
                .HasDatabaseName("IX_StoryViews_UserId");

            builder.HasIndex(sv => new { sv.StoryId, sv.UserId })
                .HasDatabaseName("IX_StoryViews_StoryId_UserId");
        }
    }
}
