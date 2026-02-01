using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Result.Abstrack;

namespace Core.Result.Concret
{
    public abstract class Result : IResult
    {
        public string Message { get; }

        public bool IsSuccess {  get; }
        public Result(bool isSuccess)
        {
            IsSuccess=isSuccess;
        }
        public Result(string message,bool isSuccess):this(isSuccess)
        {
            Message=message;
        }
    }
}
