using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.Models
{
    public class GroupMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool IsMuted { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Group Group { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
