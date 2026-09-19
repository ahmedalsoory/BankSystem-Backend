using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public abstract class BaseService<T>
    {
        protected readonly ILogger<T> _logger;

        protected BaseService(ILogger<T> logger)
        {
            _logger = logger;
        }

        // This is the ONLY place where you handle repetitive DB infrastructure noise
        protected async Task<OperationResult> ExecuteDbOperationAsync(
            Func<Task<OperationResult>> operation,
            string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Log for internal tracing (Middleware won't see this)
                _logger.LogWarning(ex, "Business Rule Violation: Unique constraint conflict.");

                // Return friendly business failure
                return OperationResult.Failure(errorMessage);
            }
        }
        protected async Task<OperationResult<T>> ExecuteDbOperationAsync<T>(
        Func<Task<OperationResult<T>>> operation,
        string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                _logger.LogWarning(ex, "Business Rule Violation: Unique constraint conflict.");
                return OperationResult<T>.Failure(errorMessage);
            }
        }
        //protected async Task<T> ExecuteDbOperationAsync(
        //  Func<Task<T>> operation
        //  )
        //{
        //    try
        //    {
        //        return await operation();
        //    }
        //    catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        //    {
        //        // Log for internal tracing (Middleware won't see this)
        //        _logger.LogWarning(ex, "Business Rule Violation: Unique constraint conflict.");

        //        // Return friendly business failure

        //    }

        //}
     
    }
}
