using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Shared.Cache;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace API.Filter
{
    public class CacheVersionFilter : IAsyncResourceFilter
    {
        private readonly ICacheVersionService _versionService;

        public CacheVersionFilter(ICacheVersionService versionService) => _versionService = versionService;

        private static string GetResourceName(ResourceExecutingContext context)
        {
            var pathSegments = context.HttpContext.Request.Path.Value?
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (pathSegments == null || pathSegments.Length == 0)
            {
                return "default";
            }

            int startIndex = pathSegments[0].Equals("api", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

            if (startIndex >= pathSegments.Length)
            {
                return "default";
            }

            return string.Join("_", pathSegments.Skip(startIndex)).ToLower();
        }

        private static string GetDomainPrefix(string resourceName)
        {
            // Extracts the root domain prefix (e.g., "account_paged" -> "account")
            var parts = resourceName.Split('_', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : resourceName;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (next != null)
            {
                ArgumentNullException.ThrowIfNull(context);
                string resourceName = GetResourceName(context);

                // 1. Mutations (POST, PUT, DELETE) -> Invalidate via Prefix so all related lists drop
                if (!HttpMethods.IsGet(context.HttpContext.Request.Method))
                {


                    var executedContext = await next().ConfigureAwait(false); ;
                    if (executedContext.Exception == null)
                    {
                        string domainPrefix = GetDomainPrefix(resourceName);

                        // Invalidate both the exact resource endpoint AND the broader domain prefix
                        _versionService.InvalidatePrefix(domainPrefix);
                    }
                    return;

                }

                // 2. Check the Controller Action's Return Type (Safely unwrapping nested generics)
                bool isPagedResult = false;
                if (context.ActionDescriptor is ControllerActionDescriptor controllerAction)
                {
                    Type actualType = controllerAction.MethodInfo.ReturnType;

                    // Recursively unwrap Task<T>, ActionResult<T>, etc., until we get the core return type
                    while (actualType.IsGenericType)
                    {
                        var genericDef = actualType.GetGenericTypeDefinition();
                        if (genericDef == typeof(Task<>) || genericDef == typeof(ActionResult<>))
                        {
                            actualType = actualType.GetGenericArguments()[0];
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (actualType.Name.Contains("PagedResult", StringComparison.OrdinalIgnoreCase))
                    {
                        isPagedResult = true;
                    }
                }

                // 3. Only apply ETag logic if it's a PagedResult
                if (isPagedResult)
                {
                    // This call automatically triggers _activeKeys.TryAdd(resourceName, 0) inside CacheVersionService
                    string currentVersion = _versionService.GetVersion(resourceName);
                    string clientETag = context.HttpContext.Request.Headers["If-None-Match"].ToString();

                    if (clientETag == $"\"{currentVersion}\"")
                    {
                        context.Result = new StatusCodeResult(304);
                        return;
                    }

                    context.HttpContext.Response.OnStarting(() =>
                    {
                        context.HttpContext.Response.Headers["ETag"] = $"\"{currentVersion}\"";
                        return Task.CompletedTask;
                    });
                }

                await next().ConfigureAwait(false);
            }
            return;
        }
    }
}