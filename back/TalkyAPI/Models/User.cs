using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Base64 encoded image - no max length (can be large)
        public string? Avatar { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        public bool IsOnline { get; set; } = false;

        public DateTime? LastSeen { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public ICollection<Call> InitiatedCalls { get; set; } = new List<Call>();
        public ICollection<Call> ReceivedCalls { get; set; } = new List<Call>();
    }
}
