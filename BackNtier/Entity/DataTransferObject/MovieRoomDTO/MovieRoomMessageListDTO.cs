using System;

namespace Entity.DataTransferObject.MovieRoomDTO
{
    public class MovieRoomMessageListDTO
    {
        public int Id { get; set; }
        public int MovieRoomId { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
