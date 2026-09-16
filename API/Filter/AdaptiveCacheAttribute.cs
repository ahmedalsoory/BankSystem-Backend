using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

/*
public class AdaptiveCacheAttribute : ActionFilterAttribute
{
    private readonly string _cacheKey;
    private readonly int _seconds;
    private readonly int _pressureThreshold;

    // Static fields shared across all instances of the attribute
    private static int _activeRequests = 0;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    private static void CleanupLocks()
    {
        // If we have more than 1000 unique locks, clear them to save RAM
        if (_locks.Count > 1000)
        {
            _locks.Clear();
        }
    }

    public AdaptiveCacheAttribute(string cacheKey, int seconds = 30, int pressureThreshold = 10)
    {
        _cacheKey = cacheKey;
        _seconds = seconds;
        _pressureThreshold = pressureThreshold;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

        // 1. GENERATE DYNAMIC KEY
        // This combines the path (/api/cabins) with the query (?page=1&sort=price)
        // Result: "shield_/api/cabins?page=1&sort=price"
        var request = context.HttpContext.Request;
        string dynamicKey = $"shield_{request.Path}{request.QueryString}";

        // 2. THE SHIELD (Fastest)
        if (cache.TryGetValue(dynamicKey, out object fastResult))
        {
            context.Result = new OkObjectResult(fastResult);
            return;
        }

        Interlocked.Increment(ref _activeRequests);
        try
        {
            // 3. OPTIMISTIC BYPASS (Low Pressure)
            if (_activeRequests <= _pressureThreshold)
            {
                var resultContext = await next();
                if (resultContext.Result is OkObjectResult ok)
                {
                    cache.Set(dynamicKey, ok.Value, TimeSpan.FromSeconds(_seconds));
                }
                return;
            }

            // 4. PROTECTIVE LOCK (High Pressure)
            // Note: The lock is now specific to the dynamicKey (the specific page/sort)
            var myLock = _locks.GetOrAdd(dynamicKey, _ => new SemaphoreSlim(1, 1));
            await myLock.WaitAsync();
            try
            {
                if (cache.TryGetValue(dynamicKey, out object lockedResult))
                {
                    context.Result = new OkObjectResult(lockedResult);
                    return;
                }

                var executedContext = await next();
                if (executedContext.Result is OkObjectResult okResult)
                {
                    cache.Set(dynamicKey, okResult.Value, TimeSpan.FromSeconds(_seconds));
                }
            }
            finally { myLock.Release(); }
        }
        finally { Interlocked.Decrement(ref _activeRequests);
            CleanupLocks();
        }
    }
}*/