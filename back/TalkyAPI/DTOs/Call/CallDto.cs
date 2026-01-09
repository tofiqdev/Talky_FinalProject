namespace TalkyAPI.DTOs.Call
{
    public class CallDto
    {
        public int Id { get; set; }
        public int CallerId { get; set; }
        public int ReceiverId { get; set; }
        public string CallerUsername { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public string CallType { get; set; } = string.Empty; // voice or video
        public string Status { get; set; } = string.Empty; // missed, completed, rejected
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? Duration { get; set; }
    }
}
