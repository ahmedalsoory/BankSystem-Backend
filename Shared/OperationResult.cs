using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public IReadOnlyList<string> Errors { get; set; } = Array.Empty<string>();

        // 🔄 Update this to accept IReadOnlyList<string>
        public static OperationResult Failure(IReadOnlyList<string> errors) =>
            new OperationResult { Success = false, Errors = errors };

        public static OperationResult Failure(string error) =>
            new OperationResult { Success = false, Errors = new[] { error } };

        public static OperationResult Ok() =>
            new OperationResult { Success = true, Errors = Array.Empty<string>() };
    }

    public class OperationResult<T> : OperationResult
    {
        public T Data { get; set; }

        public static OperationResult<T> Ok(T data) =>
            new OperationResult<T> { Success = true, Data = data };

        // 🔄 Update this to accept IReadOnlyList<string>
        public static new OperationResult<T> Failure(IReadOnlyList<string> errors) =>
            new OperationResult<T> { Success = false, Errors = errors };

        public static new OperationResult<T> Failure(string error) =>
            new OperationResult<T> { Success = false, Errors = new[] { error } };
    }
}
