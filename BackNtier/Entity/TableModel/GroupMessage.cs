using Core.Entities;
// using Entities.TableModel; removed
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.TableModel
{
    public class GroupMessage:BaseEntity
    {
        
        public int GroupId { get; set; }

        
        public int SenderId { get; set; }

        
        public string Content { get; set; } = string.Empty;

        public bool IsSystemMessage { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Group Group { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}
