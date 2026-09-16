using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class BusinessValidationException : BusinessException
    {
        // 🚀 This holds the specific database validation failure reasons
        public List<string> ValidationErrors { get; }

        public BusinessValidationException(List<string> errors)
            : base("One or more business validation errors occurred.", "VALIDATION_ERROR")
        {
            ValidationErrors = errors ?? new List<string>();
        }
    }
}
