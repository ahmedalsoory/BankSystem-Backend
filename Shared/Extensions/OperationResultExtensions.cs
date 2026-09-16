using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    /*
    public static class OperationResultExtensions
    {
        // Allows chaining async operations if the previous one succeeded
        public static async Task<OperationResult<TOut>> BindAsync<TIn, TOut>(
            this Task<OperationResult<TIn>> previousTask,
            Func<TIn, Task<OperationResult<TOut>>> nextAction)
        {
            var previousResult = await previousTask;

            if (!previousResult.Success)
            {
                return OperationResult<TOut>.Failure(previousResult.Errors);
            }

            return await nextAction(previousResult.Data);
        }

        // Allows running validation rules in a chain
        public static async Task<OperationResult<T>> EnsureAsync<T>(
            this OperationResult<T> result,
            Func<T, Task<bool>> predicate,
            string errorMessage)
        {
            if (!result.Success) return result;

            bool isValid = await predicate(result.Data);
            if (!isValid)
            {
                return OperationResult<T>.Failure(errorMessage);
            }

            return result;
        }
    }*/
}
