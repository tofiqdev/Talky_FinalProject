using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Result.Abstrack;
using Core.Result.Concret;


namespace Core.Business
{
    public class BusinessRules
    {
        public static IResult Check(params IResult[] results)
        {
            foreach (var result in results)
            {
                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            return new SuccesResult();
        }
    }

    
}
