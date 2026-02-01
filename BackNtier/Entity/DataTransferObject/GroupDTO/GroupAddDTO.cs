using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.GroupDTO
{
    
    public class GroupAddDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public int CreatedById { get; set; }
    }
}
