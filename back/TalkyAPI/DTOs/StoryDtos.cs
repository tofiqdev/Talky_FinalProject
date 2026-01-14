namespace TalkyAPI.DTOs
{
    public class CreateStoryDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
    }

    public class StoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int ViewCount { get; set; }
        public bool HasViewed { get; set; }
    }

    public class StoryViewDto
    {
        public int Id { get; set; }
        public int StoryId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; }
    }
}
