using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Base64 encoded image - no max length
        public string? Avatar { get; set; }

        [Required]
        public int CreatedById { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mute all members (only admins can speak)
        public bool IsMutedForAll { get; set; } = false;

        // Navigation properties
        public User CreatedBy { get; set; } = null!;
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<GroupMessage> Messages { get; set; } = new List<GroupMessage>();
    }
}
