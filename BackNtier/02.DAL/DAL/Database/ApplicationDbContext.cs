using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Entity.TableModel;
using Entity.TableModel.Membership;
using Entity.TableModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Database
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        // Yeni eklenen DbSet'ler
        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryView> StoryViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Identity Tablo Özelleştirmeleri
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUsers");
            });

            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.ToTable("AppRoles");
            });
            #endregion

            #region Kendi Entity'leriniz için Configuration'lar
            // Configuration'ları assembly'den otomatik yükle
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Veya manuel olarak ekleyin:
            // modelBuilder.ApplyConfiguration(new UserConfiguration());
            // modelBuilder.ApplyConfiguration(new MessageConfiguration());
            // modelBuilder.ApplyConfiguration(new CallConfiguration());
            // modelBuilder.ApplyConfiguration(new GroupConfiguration());
            // modelBuilder.ApplyConfiguration(new GroupMemberConfiguration());
            // modelBuilder.ApplyConfiguration(new GroupMessageConfiguration());
            #endregion

            #region Özel İlişkiler ve Index'ler
            // Özel ilişkileri burada da tanımlayabilirsiniz

            // User - Message ilişkileri
            modelBuilder.Entity<User>()
                .HasMany(u => u.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Call ilişkileri
            modelBuilder.Entity<User>()
                .HasMany(u => u.InitiatedCalls)
                .WithOne(c => c.Caller)
                .HasForeignKey(c => c.CallerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedCalls)
                .WithOne(c => c.Receiver)
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Group ilişkileri (eğer User entity'sinde varsa)
            // modelBuilder.Entity<User>()
            //     .HasMany(u => u.GroupMemberships)
            //     .WithOne(gm => gm.User)
            //     .HasForeignKey(gm => gm.UserId)
            //     .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Global Query Filters (Opsiyonel)
            // Tüm entity'ler için soft delete filtresi
            modelBuilder.Entity<User>().HasQueryFilter(u => u.Deleted == 0);
            modelBuilder.Entity<Message>().HasQueryFilter(m => m.Deleted == 0);
            modelBuilder.Entity<Call>().HasQueryFilter(c => c.Deleted == 0);
            modelBuilder.Entity<Group>().HasQueryFilter(g => g.Deleted == 0);
            modelBuilder.Entity<GroupMember>().HasQueryFilter(gm => gm.Deleted == 0);
            modelBuilder.Entity<GroupMessage>().HasQueryFilter(gm => gm.Deleted == 0);
            modelBuilder.Entity<BlockedUser>().HasQueryFilter(bu => bu.Deleted == 0);
            modelBuilder.Entity<Contact>().HasQueryFilter(c => c.Deleted == 0);
            modelBuilder.Entity<Story>().HasQueryFilter(s => s.Deleted == 0);
            modelBuilder.Entity<StoryView>().HasQueryFilter(sv => sv.Deleted == 0);
            #endregion
        }
    }
}