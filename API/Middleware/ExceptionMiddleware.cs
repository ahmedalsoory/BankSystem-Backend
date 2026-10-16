using Shared;
using Shared.Interfaces;
using ServiceContract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, Context errorContext, IPerformanceAlertService alertService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string sourceAction = errorContext.CurrentAction ?? "Unknown.Source";

                _logger.LogError(ex,
                    "Unhandled exception | Action: {SourceAction} | Path: {Path} | TraceId: {TraceId} | User: {UserId}",
                    sourceAction,
                    context.Request.Path,
                    context.TraceIdentifier,
                    context.User?.FindFirst("sub")?.Value ?? "Anonymous");

                try
                {
                    // Build formatted Telegram alert matching the detailed layout
                    var alertMessage = new StringBuilder()
                        .AppendLine($"• *Path:* `{context.Request.Method} {context.Request.Path}`")
                        .AppendLine($"• *Action:* `{sourceAction}`")
                        .AppendLine($"• *Exception:* `{ex.GetType().Name}`")
                        .AppendLine($"• *Message:* `{ex.Message}`")
                        .AppendLine($"• *TraceId:* `{context.TraceIdentifier}`")
                        .AppendLine()
                        .AppendLine($"• *Time (UTC):* {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}")
                        .ToString();

                    await alertService.SendErrorAlertAsync("Unhandled Server Exception!", alertMessage);
                }
                catch (Exception alertEx)
                {
                    _logger.LogError(alertEx, "Failed to send performance alert via IPerformanceAlertService.");
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return Task.CompletedTask;
            }

            context.Response.ContentType = "application/json";

            int statusCode = 500;
            string message = exception.Message;
            string errorCode = "SERVER_ERROR";

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = message,
                TraceId = context.TraceIdentifier
            });
        }
    }
}