using Microsoft.EntityFrameworkCore;
using TalkyAPI.Models;

namespace TalkyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryView> StoryViews { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Message entity configuration
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(e => e.SenderId);
                entity.HasIndex(e => e.ReceiverId);
                entity.HasIndex(e => e.SentAt);

                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(m => m.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Call entity configuration
            modelBuilder.Entity<Call>(entity =>
            {
                entity.HasIndex(e => e.CallerId);
                entity.HasIndex(e => e.ReceiverId);
                entity.HasIndex(e => e.StartedAt);

                entity.HasOne(c => c.Caller)
                    .WithMany(u => u.InitiatedCalls)
                    .HasForeignKey(c => c.CallerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Receiver)
                    .WithMany(u => u.ReceivedCalls)
                    .HasForeignKey(c => c.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Group entity configuration
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasIndex(e => e.CreatedById);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(g => g.CreatedBy)
                    .WithMany()
                    .HasForeignKey(g => g.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // GroupMember entity configuration
            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();

                entity.HasOne(gm => gm.Group)
                    .WithMany(g => g.Members)
                    .HasForeignKey(gm => gm.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gm => gm.User)
                    .WithMany()
                    .HasForeignKey(gm => gm.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // GroupMessage entity configuration
            modelBuilder.Entity<GroupMessage>(entity =>
            {
                entity.HasIndex(e => e.GroupId);
                entity.HasIndex(e => e.SenderId);
                entity.HasIndex(e => e.SentAt);

                entity.HasOne(gm => gm.Group)
                    .WithMany(g => g.Messages)
                    .HasForeignKey(gm => gm.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gm => gm.Sender)
                    .WithMany()
                    .HasForeignKey(gm => gm.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Story entity configuration
            modelBuilder.Entity<Story>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.ExpiresAt);

                entity.HasOne(s => s.User)
                    .WithMany()
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // StoryView entity configuration
            modelBuilder.Entity<StoryView>(entity =>
            {
                entity.HasIndex(e => new { e.StoryId, e.UserId }).IsUnique();
                entity.HasIndex(e => e.ViewedAt);

                entity.HasOne(sv => sv.Story)
                    .WithMany(s => s.Views)
                    .HasForeignKey(sv => sv.StoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sv => sv.User)
                    .WithMany()
                    .HasForeignKey(sv => sv.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BlockedUser entity configuration
            modelBuilder.Entity<BlockedUser>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.BlockedUserId }).IsUnique();
                entity.HasIndex(e => e.BlockedAt);

                entity.HasOne(bu => bu.User)
                    .WithMany()
                    .HasForeignKey(bu => bu.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bu => bu.BlockedUserNavigation)
                    .WithMany()
                    .HasForeignKey(bu => bu.BlockedUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Contact entity configuration
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.ContactUserId }).IsUnique();
                entity.HasIndex(e => e.AddedAt);

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ContactUser)
                    .WithMany()
                    .HasForeignKey(c => c.ContactUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
