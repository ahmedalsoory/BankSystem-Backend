using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Polly;
using Shared;
using Shared.Interfaces;

namespace API.Filter
{
    public sealed class GlobalConnectionFilter : IAsyncActionFilter
    {
        private readonly IDbContextScope _dbScope;
        public GlobalConnectionFilter(IDbContextScope dbScope) => _dbScope = dbScope;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Define the Polly retry policy using your existing IsTransient logic or standard SQL 1205 check
            var retryPolicy = Policy
                .Handle<SqlException>(ex => ex.Number == 1205 && _dbScope.CurrentTransaction != null) // Or use your transient checker
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(50 * retryAttempt));

            // Wrap the entire request execution in the retry policy!
            await retryPolicy.ExecuteAsync(async () =>
            {
                // Ensure fresh state on every retry attempt
                await _dbScope.DisposeAsync();

                var executedContext = await next();

                if (_dbScope.CurrentTransaction != null)
                {
                    if (executedContext.Result is ObjectResult obj && obj.Value is OperationResult result && result.Success)
                    {
                        _dbScope.Commit();
                    }
                    else
                    {
                        _dbScope.Rollback();
                    }
                }

                _dbScope.CloseConnection();
            });
        }
    }
}

