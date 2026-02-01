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
    public class GroupMember:BaseEntity
    {
        
        

        
        public int GroupId { get; set; }

        
        public int UserId { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool IsMuted { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Group Group { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
