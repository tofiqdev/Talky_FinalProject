using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.DTOs.Group
{
    public class CreateGroupDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public List<int> MemberIds { get; set; } = new List<int>();
    }
}
