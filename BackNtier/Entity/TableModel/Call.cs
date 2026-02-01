using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Entity.TableModel
{
    public class Call:BaseEntity
    {
        public int CallerId { get; set; }
        public int ReceiverId { get; set; }
        public string CallType { get; set; } = "voice";
        public string Status { get; set; } = "completed";
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public int? Duration { get; set; }
        public User Caller { get; set; } = null!;
        public User Receiver { get; set; } = null!;

    }
}
