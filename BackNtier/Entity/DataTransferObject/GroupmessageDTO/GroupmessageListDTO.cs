using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.GroupmessageDTO
{
    public class GroupmessageListDTO
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public bool IsSystemMessage { get; set; }
        public DateTime SentAt { get; set; }
    }
}
