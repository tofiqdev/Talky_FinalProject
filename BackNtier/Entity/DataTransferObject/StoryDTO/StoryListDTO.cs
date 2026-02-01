using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.StoryDTO
{
    public class StoryListDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? UserAvatar { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int ViewCount { get; set; }
        public bool HasViewed { get; set; }
        public bool IsExpired => ExpiresAt < DateTime.UtcNow;
    }
}
