using System;

namespace Entity.DataTransferObject.MovieRoomDTO
{
    public class MovieRoomParticipantListDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public bool IsOnline { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
