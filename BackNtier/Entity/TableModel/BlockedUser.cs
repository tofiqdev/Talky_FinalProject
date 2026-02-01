using Core.Entities;
// using Entities.TableModel; removed
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.TableModel
{
    public class BlockedUser : BaseEntity
    {
        public int UserId { get; set; } // Who blocked
        public int BlockedUserId { get; set; } // Who is blocked
        public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public User BlockedUserNavigation { get; set; } = null!;
    }
}
