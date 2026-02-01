using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.GroupmessageDTO
{
    public class GroupmessageAddDTO
    {
        public int GroupId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public bool IsSystemMessage { get; set; } = false;
    }
}
