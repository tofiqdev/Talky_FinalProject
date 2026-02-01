using Core.Entities;
using System;

namespace Entity.TableModel
{
    public class MovieRoomMessage : BaseEntity
    {
        public int MovieRoomId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public MovieRoom MovieRoom { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}
