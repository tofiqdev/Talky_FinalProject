using System.ComponentModel.DataAnnotations;

namespace TalkyAPI.DTOs.Call
{
    public class CreateCallDto
    {
        [Required(ErrorMessage = "Receiver ID is required")]
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "Call type is required")]
        [RegularExpression("^(voice|video)$", ErrorMessage = "Call type must be 'voice' or 'video'")]
        public string CallType { get; set; } = "voice";

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(missed|completed|rejected)$", ErrorMessage = "Status must be 'missed', 'completed', or 'rejected'")]
        public string Status { get; set; } = "completed";

        public int? Duration { get; set; } // in seconds
    }
}
