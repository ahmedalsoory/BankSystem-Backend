using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class ProtocolCheckMiddleware
{
    private readonly RequestDelegate _next;

    public ProtocolCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Enforce modern HTTP protocols only
        var protocol = context.Request.Protocol;
        if (!protocol.StartsWith("HTTP/1.1", StringComparison.OrdinalIgnoreCase) &&
            !protocol.StartsWith("HTTP/2", StringComparison.OrdinalIgnoreCase) &&
            !protocol.StartsWith("HTTP/3", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Unsupported or unsafe protocol.");
            return;
        }

        await _next(context);
    }
}

