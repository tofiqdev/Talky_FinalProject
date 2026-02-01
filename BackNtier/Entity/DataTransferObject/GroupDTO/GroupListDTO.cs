using Entity.DataTransferObject.GroupMenmberDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.GroupDTO
{
    public class GroupListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByUsername { get; set; }
        public bool IsMutedForAll { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public List<GroupMemberListDTO> Members { get; set; } = new List<GroupMemberListDTO>();
    }
}
