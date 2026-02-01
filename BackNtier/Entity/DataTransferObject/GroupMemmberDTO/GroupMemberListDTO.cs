using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.GroupMenmberDTO
{
    public class GroupMemberListDTO
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? Avatar { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsMuted { get; set; }
        public bool IsOnline { get; set; }
        public DateTime JoinedAt { get; set; }
    }

}
