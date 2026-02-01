using Core.Entities;
using System;
using System.Collections.Generic;

namespace Entity.TableModel
{
    public class MovieRoom : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string YouTubeUrl { get; set; } = string.Empty;
        public string YouTubeVideoId { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public bool IsActive { get; set; } = true;
        public double CurrentTime { get; set; } = 0; // Video current time in seconds
        public bool IsPlaying { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User CreatedBy { get; set; } = null!;
        public ICollection<MovieRoomParticipant> Participants { get; set; } = new List<MovieRoomParticipant>();
        public ICollection<MovieRoomMessage> Messages { get; set; } = new List<MovieRoomMessage>();
    }
}
