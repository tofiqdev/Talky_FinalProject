using System;
using System.Collections.Generic;

namespace Entity.DataTransferObject.MovieRoomDTO
{
    public class MovieRoomListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string YouTubeUrl { get; set; } = string.Empty;
        public string YouTubeVideoId { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public string? CreatedByAvatar { get; set; }
        public bool IsActive { get; set; }
        public double CurrentTime { get; set; }
        public bool IsPlaying { get; set; }
        public int ParticipantCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MovieRoomParticipantListDTO> Participants { get; set; } = new List<MovieRoomParticipantListDTO>();
    }
}
