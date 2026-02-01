using Core.Entities;
using System;

namespace Entity.TableModel
{
    public class MovieRoomParticipant : BaseEntity
    {
        public int MovieRoomId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public MovieRoom MovieRoom { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
