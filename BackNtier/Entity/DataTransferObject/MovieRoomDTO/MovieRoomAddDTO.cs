namespace Entity.DataTransferObject.MovieRoomDTO
{
    public class MovieRoomAddDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string YouTubeUrl { get; set; } = string.Empty;
        public int CreatedById { get; set; }
    }
}
