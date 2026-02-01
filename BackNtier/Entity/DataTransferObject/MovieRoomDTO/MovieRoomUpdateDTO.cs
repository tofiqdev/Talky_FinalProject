namespace Entity.DataTransferObject.MovieRoomDTO
{
    public class MovieRoomUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string YouTubeUrl { get; set; } = string.Empty;
        public string YouTubeVideoId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public double CurrentTime { get; set; }
        public bool IsPlaying { get; set; }
    }
}
