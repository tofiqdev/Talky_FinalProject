using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Extension
{
    public static class ErrorValidationToString
    {
        internal static string FluentErrorString(this List<ValidationFailure> failures)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var sb in failures)
            {
                stringBuilder.Append(sb);
                stringBuilder.Append(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
    }
}
