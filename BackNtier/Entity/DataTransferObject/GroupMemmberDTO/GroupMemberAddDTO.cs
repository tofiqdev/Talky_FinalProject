using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.GroupMenmberDTO
{
    public class GroupMemberAddDTO
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
