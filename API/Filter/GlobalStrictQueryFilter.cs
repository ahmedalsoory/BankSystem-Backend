using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace API.Filter
{
    public class GlobalStrictQueryFilter : IAsyncActionFilter
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _cache = new();

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Quick exit if no query parameters
            if (context.HttpContext.Request.Query.Count == 0)
            {
                await next();
                return;
            }

            // 2. Optimized retrieval from cache
            string cacheKey = context.ActionDescriptor.Id;
            if (!_cache.TryGetValue(cacheKey, out var allowedKeys))
            {
                allowedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                // Only here does reflection happen, and only ONCE
                foreach (var param in context.ActionDescriptor.Parameters)
                {
                    if (param.ParameterType.IsPrimitive || param.ParameterType == typeof(string) || param.ParameterType.IsValueType)
                        allowedKeys.Add(param.Name);
                    else
                        foreach (var prop in param.ParameterType.GetProperties())
                            allowedKeys.Add(prop.Name);
                }
                _cache.TryAdd(cacheKey, allowedKeys);
            }

            // 3. Fast O(1) validation
            foreach (var key in context.HttpContext.Request.Query.Keys)
            {
                if (!allowedKeys.Contains(key))
                {
                    context.Result = new BadRequestObjectResult("Invalid parameter: " + key);
                    return;
                }
            }

            await next();
        }
    }
}
