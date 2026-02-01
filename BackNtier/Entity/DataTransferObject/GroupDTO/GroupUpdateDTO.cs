using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.GroupDTO
{
    public class GroupUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public bool IsMutedForAll { get; set; }
    }
}
