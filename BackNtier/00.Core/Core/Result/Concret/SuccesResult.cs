using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Result.Concret
{
    public class SuccesResult:Result
    {
        public SuccesResult():base(true)
        {
            
        }
        public SuccesResult(string message) : base(message, true)
        {

        }
    }
}
