using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalkyAPI.Models
{
    public class Call
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CallerId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [MaxLength(20)]
        public string CallType { get; set; } = "voice"; // voice or video

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "completed"; // missed, completed, rejected

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? EndedAt { get; set; }

        public int? Duration { get; set; } // in seconds

        // Navigation properties
        [ForeignKey("CallerId")]
        public User Caller { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } = null!;
    }
}
