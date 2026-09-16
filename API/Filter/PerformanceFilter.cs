using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;



namespace API.Filter
{
   
    public class PerformanceFilter : IAsyncActionFilter
    {
        private readonly ILogger<PerformanceFilter> _logger;

        public PerformanceFilter(ILogger<PerformanceFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var resultContext = await next();
            stopwatch.Stop();

            // Attach a "Category" property to the log event
            _logger.LogInformation(
       "Performance logs: {Endpoint} took {DurationMs} ms at {CreatedDate}",
       $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}", // Argument 1
       stopwatch.ElapsedMilliseconds,                                            // Argument 2
       DateTime.UtcNow                                                           // Argument 3
   );
        }
    }
}
