using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.Models
{
    public class GroupMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Group Group { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}
