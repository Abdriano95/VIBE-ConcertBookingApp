using System.Net;
using System.Text.Json;
using DMA_AU24_LAB2_Group4.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware that catches all unhandled exceptions
    /// and returns a consistent JSON error response.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            var path = context.Request.Path.Value;

            // Log the exception with full details
            _logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Message: {Message}",
                traceId,
                path,
                exception.Message);

            // Determine status code and message based on exception type
            var (statusCode, message) = GetStatusCodeAndMessage(exception);

            // Build the error response
            var errorResponse = ApiErrorResponse.Create(
                statusCode: statusCode,
                message: message,
                details: _environment.IsDevelopment() ? exception.Message : null,
                stackTrace: _environment.IsDevelopment() ? exception.StackTrace : null,
                path: path,
                traceId: traceId
            );

            // Configure the response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            // Write the JSON response
            var json = JsonSerializer.Serialize(errorResponse, JsonOptions);
            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Maps exception types to HTTP status codes and user-friendly messages.
        /// </summary>
        private static (int StatusCode, string Message) GetStatusCodeAndMessage(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => 
                    ((int)HttpStatusCode.BadRequest, "A required parameter was not provided."),
                
                ArgumentException => 
                    ((int)HttpStatusCode.BadRequest, "Invalid argument provided."),
                
                KeyNotFoundException => 
                    ((int)HttpStatusCode.NotFound, "The requested resource was not found."),
                
                UnauthorizedAccessException => 
                    ((int)HttpStatusCode.Unauthorized, "Access denied. You are not authorized to perform this action."),
                
                InvalidOperationException => 
                    ((int)HttpStatusCode.Conflict, "The operation is not valid in the current state."),
                
                NotImplementedException => 
                    ((int)HttpStatusCode.NotImplemented, "This feature is not yet implemented."),
                
                TimeoutException => 
                    ((int)HttpStatusCode.RequestTimeout, "The operation timed out. Please try again."),
                
                OperationCanceledException => 
                    ((int)HttpStatusCode.BadRequest, "The operation was cancelled."),
                
                // Database-related exceptions (Entity Framework)
                // Note: DbUpdateConcurrencyException must come before DbUpdateException (inheritance)
                Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException => 
                    ((int)HttpStatusCode.Conflict, "A concurrency conflict occurred. The data may have been modified by another user."),
                
                Microsoft.EntityFrameworkCore.DbUpdateException => 
                    ((int)HttpStatusCode.BadRequest, "A database error occurred while saving changes."),
                
                // Default: Internal Server Error
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
            };
        }
    }

    /// <summary>
    /// Extension methods for registering the exception middleware.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Adds the global exception handling middleware to the application pipeline.
        /// This should be added early in the pipeline to catch all exceptions.
        /// </summary>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
