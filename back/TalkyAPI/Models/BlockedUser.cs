namespace TalkyAPI.Models
{
    public class BlockedUser
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Who blocked
        public int BlockedUserId { get; set; } // Who is blocked
        public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public User User { get; set; } = null!;
        public User BlockedUserNavigation { get; set; } = null!;
    }
}
