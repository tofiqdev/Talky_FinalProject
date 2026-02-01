using Core.Entities;
// using Entities.TableModel; removed
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.TableModel
{
    public class Story : BaseEntity
    {
        public int UserId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);

        // Navigation
        public User User { get; set; } = null!;
        public ICollection<StoryView> Views { get; set; } = new List<StoryView>();
    }
}
