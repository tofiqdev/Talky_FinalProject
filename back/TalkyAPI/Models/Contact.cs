using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ContactUserId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public User ContactUser { get; set; } = null!;
    }
}
