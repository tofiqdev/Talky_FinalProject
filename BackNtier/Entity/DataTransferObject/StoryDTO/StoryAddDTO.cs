using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.StoryDTO
{
    public class StoryAddDTO
    {
        public int UserId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
