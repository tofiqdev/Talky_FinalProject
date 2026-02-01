using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Result.Concret
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        private string v;

        public ErrorDataResult(T data) : base(data, false)
        {

        }

        // Fix: Call base constructor with appropriate parameters
        public ErrorDataResult(string v) : base(default(T), v, false)
        {
            this.v = v;
        }

        public ErrorDataResult(T data, string message) : base(data, message, false)
        {

        }
    }
}
