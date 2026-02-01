using Core.Entities;
// using Entities.TableModel; removed
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.TableModel
{
    public class StoryView : BaseEntity
    {
        public int StoryId { get; set; }
        public int UserId { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Story Story { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
