using Core.Entities;
// using Entities.TableModel; removed
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.TableModel
{
    public class Contact : BaseEntity
    {
        public int UserId { get; set; }
        public int ContactUserId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public User ContactUser { get; set; } = null!;
    }
}
