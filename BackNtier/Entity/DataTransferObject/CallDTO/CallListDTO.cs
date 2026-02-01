using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.CallDTO
{
    public class CallListDTO
    {
        public int Id { get; set; }
        public int CallerId { get; set; }
        public string CallerName { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string CallType { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? Duration { get; set; }
    }
}
