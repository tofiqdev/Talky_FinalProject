using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.CallDTO
{
    public class CallUpdateDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? Duration { get; set; }
    }
}
