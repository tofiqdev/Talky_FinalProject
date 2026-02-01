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
    public class BlockedUserConfiguration : IEntityTypeConfiguration<BlockedUser>
    {
        public void Configure(EntityTypeBuilder<BlockedUser> builder)
        {
            builder.ToTable("BlockedUsers");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.UserId)
                .IsRequired();

            builder.Property(b => b.BlockedUserId)
                .IsRequired();

            builder.Property(b => b.BlockedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.BlockedUserNavigation)
                .WithMany()
                .HasForeignKey(b => b.BlockedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(b => new { b.UserId, b.BlockedUserId })
                .IsUnique()
                .HasDatabaseName("IX_BlockedUsers_UserId_BlockedUserId");

            builder.HasIndex(b => b.UserId)
                .HasDatabaseName("IX_BlockedUsers_UserId");

            builder.HasIndex(b => b.BlockedUserId)
                .HasDatabaseName("IX_BlockedUsers_BlockedUserId");
        }
    }
}
