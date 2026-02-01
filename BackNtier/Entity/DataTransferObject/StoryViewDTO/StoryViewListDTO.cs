using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.StoryViewDTO
{
    public class StoryViewListDTO
    {
        public int Id { get; set; }
        public int StoryId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? UserAvatar { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
