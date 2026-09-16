using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MyControllerBase : ControllerBase
    {
        // Define the compiled LoggerMessage delegate as a static field
        private static readonly Action<ILogger, long, Exception?> _performanceLog =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new EventId(1, "PerformanceExecutionTime"),
                "[PERFORMANCE LOG] Execution took: {Elapsed} ms");

        private readonly ILogger Logger;

        protected MyControllerBase(ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(loggerFactory);
            Logger = loggerFactory.CreateLogger(GetType());
        }

        protected async Task<T> ExecutePerformanceTimed<T>(Func<Task<T>> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            var stopwatch = Stopwatch.StartNew();
            var result = await action().ConfigureAwait(false); // Execute the service call
            stopwatch.Stop();

            // Log and Header logic
            var elapsed = stopwatch.ElapsedMilliseconds;
            Response.Headers["X-Database-Execution-Time-MS"] = elapsed.ToString(CultureInfo.InvariantCulture);

            // Use the high-performance compiled logger delegate
            _performanceLog(Logger, elapsed, null);

            return result; // Return the raw data
        }
    }
}