using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }

        public BusinessException(string message, string errorCode = "BUSINESS_RULE_VIOLATION")
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string resourceName, object key)
            : base($"{resourceName} with ID '{key}' was not found.") { }

        public NotFoundException(string message) : base(message) { }
    }

    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(
            string message = "Data was modified by another process. Please retry.")
            : base(message) { }
    }
}
