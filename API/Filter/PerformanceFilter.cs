using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using ServiceContract; // Ensure this matches your namespace for IPerformanceAlertService

namespace API.Filter
{
    public class PerformanceFilter : IAsyncActionFilter
    {
        private readonly ILogger<PerformanceFilter> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IPerformanceAlertService _alertService;

        // Threshold in milliseconds (e.g., 1000ms = 1 second)
        private const long ThresholdMs = 1000;

        public PerformanceFilter(
            ILogger<PerformanceFilter> logger,
            IWebHostEnvironment env,
            IPerformanceAlertService alertService)
        {
            _logger = logger;
            _env = env;
            _alertService = alertService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();

            // Let the API endpoint execute normally
            var resultContext = await next();

            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var endpoint = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

            // 1. Always log locally
            _logger.LogInformation(
                "Performance logs: {Endpoint} took {DurationMs} ms at {CreatedDate}",
                endpoint, elapsedMs, DateTime.UtcNow);

            // 2. Check environment and threshold before alerting
            bool isDebugOrDevelopment = _env.IsDevelopment() || Debugger.IsAttached;

            if (!isDebugOrDevelopment && elapsedMs > ThresholdMs)
            {
                // ** THIS IS WHERE IT IS CALLED **
                // Offloaded to the ThreadPool so the client response is not delayed
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Calling your Telegram service method here!
                        await _alertService.SendSlowEndpointAlertAsync(endpoint, elapsedMs);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to invoke performance alert for {Endpoint}.", endpoint);
                    }
                });
            }
        }
    }
}