using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Polly;
using Shared;
using Shared.Image;
using Shared.Interfaces;

namespace API.Filter
{
    public sealed class GlobalConnectionFilter : IAsyncActionFilter
    {
        private readonly IDbContextScope _dbScope;
        private readonly IImageHandler _imageHandler;
        private readonly IWebHostEnvironment _env;

        public GlobalConnectionFilter(
            IDbContextScope dbScope,
            IImageHandler imageHandler,
            IWebHostEnvironment env)
        {
            _dbScope = dbScope;
            _imageHandler = imageHandler;
            _env = env;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Polly retry policy for deadlocks (1205) and timeouts (-2, 1222)
            var retryPolicy = Policy
                .Handle<SqlException>(ex =>
                    (ex.Number == 1205 || ex.Number == -2 || ex.Number == 1222) && _dbScope.CurrentTransaction != null)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(100 * retryAttempt));

            await retryPolicy.ExecuteAsync(async () =>
            {
                await _dbScope.DisposeAsync();

                try
                {
                    var executedContext = await next();

                    // Check if an unhandled exception happened during action execution
                    if (executedContext.Exception != null && !executedContext.ExceptionHandled)
                    {
                        HandleRollbackAndCleanup();
                        return;
                    }

                    if (_dbScope.CurrentTransaction != null)
                    {
                        if (executedContext.Result is ObjectResult obj &&
                            obj.Value is OperationResult result &&
                            result.Success)
                        {
                            _dbScope.Commit();
                        }
                        else
                        {
                            HandleRollbackAndCleanup();
                        }
                    }
                }
                catch (Exception)
                {
                    // Handles exceptions thrown outside executedContext (e.g., inside other filters)
                    HandleRollbackAndCleanup();
                    throw; // Rethrow to preserve global error handling
                }
                finally
                {
                    _dbScope.CloseConnection();
                }
            });
        }

        private void HandleRollbackAndCleanup()
        {
            if (_dbScope.CurrentTransaction != null)
            {
                _dbScope.Rollback();
            }

            // Cleanup any temporary/new images created during this failed request
            _imageHandler.CleanupNewImages(_env.WebRootPath);
        }
    }
}