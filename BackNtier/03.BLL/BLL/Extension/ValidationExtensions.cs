using FluentValidation.Results;
using System.Text;

namespace BLL.Extension
{
    public static class ValidationExtensions
    {
        public static string ConvertToString(this ValidationResult result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var failure in result.Errors)
            {
                stringBuilder.Append(failure.ErrorMessage);
                stringBuilder.Append(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
    }
}
