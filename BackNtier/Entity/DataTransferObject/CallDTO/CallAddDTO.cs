using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.CallDTO
{
    public class CallAddDTO
    {
        public int CallerId { get; set; }
        public int ReceiverId { get; set; }
        public string CallType { get; set; } = "voice";
    }
}
