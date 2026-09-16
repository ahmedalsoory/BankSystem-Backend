using API.Attributes;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContract;
using System.Linq;
/*
public class UniversalShieldFilter : IAsyncActionFilter
{
    private readonly IEntityCache _cache;

    public UniversalShieldFilter(IEntityCache cache) => _cache = cache;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var shieldAttr = context.ActionDescriptor.EndpointMetadata
         .OfType<ShieldEntityAttribute>()
         .FirstOrDefault();

        if (shieldAttr != null)
        {
            // Directly grab the "id" from the route parameters
            if (context.ActionArguments.TryGetValue("id", out var value) && value is int id)
            {
                if (!_cache.IsActive(shieldAttr.TargetCache, id))
                {
                    context.Result = new NotFoundObjectResult(new { error = "Not Found" });
                    return;
                }
            }
        }
        else
        {
            // Safety: If the shield is on, but no ID was provided, block the request.
            context.Result = new BadRequestObjectResult(new { error = "ID parameter is required." });
            return;
        }

        await next();
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ShieldEntityAttribute : Attribute
{
    public CacheType TargetCache { get; }

    public ShieldEntityAttribute(CacheType targetCache)
    {
        TargetCache = targetCache;
    }
} */