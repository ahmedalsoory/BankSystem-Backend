using Shared;
using Shared.Exceptions;
using Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient; // 🚀 Added to parse native SQL Server exception codes
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

        public async Task InvokeAsync(HttpContext context, Context errorContext
           )
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Ensure any active locks or transactions are instantly terminated/rolled back
    

                string sourceAction = errorContext.CurrentAction ?? "Unknown.Source";

                _logger.LogError(ex,
                    "Unhandled exception | Action: {SourceAction} | Path: {Path} | TraceId: {TraceId} | User: {UserId}",
                    sourceAction,
                    context.Request.Path,
                    context.TraceIdentifier,
                    context.User?.FindFirst("sub")?.Value ?? "Anonymous");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            if (context.Response.HasStarted)
            {
                // If headers are already sent, we cannot change content type or status code.
                // We must log and let the request finish.
                return Task.CompletedTask;
            }

            context.Response.ContentType = "application/json";

            // 🚀 CRITICAL UPDATE: Extract information if the exception is a native SQL Server Engine error
            SqlException? sqlException = FindSqlException(exception);

            int statusCode;
            string message;
            string errorCode = "SERVER_ERROR";
            List<string>? errorList = null;

            // 1. Process Database-Level Relational Constraint Rejections First
            if (sqlException != null && sqlException.Number == 547)
            {
                // 🛡️ Error 547 = Foreign Key / Check Constraint Violation.
                // This intercepts the database-level rejection of a missing Client ID or invalid reference data.
                statusCode = 400;
                errorCode = "VALIDATION_ERROR";
                message = "Validation Failed: The provided reference data or Client ID does not exist in the system.";
                errorList = new List<string> { "The parent entity relationship could not be verified on the server database constraint." };
            }
            // 2. Fall back to your standard application-level exception matching patterns
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

            // 3. Construct the unified payload response contract for your React frontend layout dashboard
            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = message,
                Errors = errorList,
                TraceId = context.TraceIdentifier
            });
        }

        /// <summary>
        /// Helper function to unpack exceptions to find an underlying SqlException.
        /// Useful if Dapper wraps a database exception inside an AggregateException or TargetInvocationException.
        /// </summary>
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