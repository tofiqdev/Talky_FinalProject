using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.DTOs.Message
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "Receiver ID is required")]
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "Message content is required")]
        [MinLength(1, ErrorMessage = "Message cannot be empty")]
        public string Content { get; set; } = string.Empty;
    }
}
