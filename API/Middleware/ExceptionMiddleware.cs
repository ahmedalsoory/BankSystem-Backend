using Shared;
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

               
            }
        }

   
    }
}