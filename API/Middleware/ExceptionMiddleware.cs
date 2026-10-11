using Shared;
using Shared.Exceptions;
using Shared.Interfaces;
using ServiceContract; // 🚀 Added to bring in IPerformanceAlertService
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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

        // 🚀 Added IPerformanceAlertService via method injection (ideal for scoped/transient services in middleware)
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

                // 🚀 Determine if this exception warrants a Telegram alert (e.g., Server Errors / 500s)
                // We typically skip alerting for routine client validation or not found exceptions.
                bool isServerOrUnexpectedError = !(ex is BusinessException || ex is NotFoundException || ex is UnauthorizedAccessException || ex is ConcurrencyException);

                if (isServerOrUnexpectedError)
                {
                    // Offload to background ThreadPool so user response latency is unaffected
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            string errorDetails =
                                $"• *Path:* `{context.Request.Method} {context.Request.Path}`\n" +
                                $"• *Action:* `{sourceAction}`\n" +
                                $"• *Exception:* `{ex.GetType().Name}`\n" +
                                $"• *Message:* `{ex.Message}`\n" +
                                $"• *TraceId:* `{context.TraceIdentifier}`";

                            await alertService.SendErrorAlertAsync("🚨 Unhandled Server Exception!", errorDetails);
                        }
                        catch (Exception alertEx)
                        {
                            _logger.LogError(alertEx, "Failed to send error alert to Telegram.");
                        }
                    });
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

            SqlException? sqlException = FindSqlException(exception);

            int statusCode;
            string message;
            string errorCode = "SERVER_ERROR";
            List<string>? errorList = null;

            if (sqlException != null && sqlException.Number == 547)
            {
                statusCode = 400;
                errorCode = "VALIDATION_ERROR";
                message = "Validation Failed: The provided reference data or Client ID does not exist in the system.";
                errorList = new List<string> { "The parent entity relationship could not be verified on the server database constraint." };
            }
            else
            {
                (statusCode, message) = exception switch
                {
                    BusinessException => (400, exception.Message),
                    NotFoundException => (404, exception.Message),
                    UnauthorizedAccessException => (401, "Unauthorized access."),
                    ConcurrencyException => (409, exception.Message),
                    _ => (500, "A server error occurred.")
                };

                if (exception is BusinessException busEx)
                {
                    errorCode = busEx.ErrorCode;
                }

                if (exception is BusinessValidationException validationEx)
                {
                    errorList = validationEx.ValidationErrors;
                }
            }

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = message,
                Errors = errorList,
                TraceId = context.TraceIdentifier
            });
        }

        private static SqlException? FindSqlException(Exception? ex)
        {
            while (ex != null)
            {
                if (ex is SqlException sqlEx) return sqlEx;
                ex = ex.InnerException;
            }
            return null;
        }
    }
}