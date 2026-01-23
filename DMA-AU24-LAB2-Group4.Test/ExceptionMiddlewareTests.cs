using System.Net;
using System.Text.Json;
using DMA_AU24_LAB2_Group4.API.Middleware;
using DMA_AU24_LAB2_Group4.Data.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DMA_AU24_LAB2_Group4.Test
{
    /// <summary>
    /// Comprehensive tests for the ExceptionMiddleware global error handling.
    /// </summary>
    public class ExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionMiddleware>> _mockLogger;
        private readonly Mock<IHostEnvironment> _mockEnvironment;

        public ExceptionMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
            _mockEnvironment = new Mock<IHostEnvironment>();
        }

        #region Helper Methods

        private ExceptionMiddleware CreateMiddleware(RequestDelegate next, bool isDevelopment = false)
        {
            _mockEnvironment.Setup(e => e.EnvironmentName)
                .Returns(isDevelopment ? Environments.Development : Environments.Production);

            return new ExceptionMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/test";
            context.TraceIdentifier = "test-trace-id";
            return context;
        }

        private static async Task<ApiErrorResponse?> GetErrorResponse(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            var json = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<ApiErrorResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        #endregion

        #region ApiErrorResponse DTO Tests

        [Fact]
        public void ApiErrorResponse_Create_SetsAllProperties()
        {
            // Arrange & Act
            var response = ApiErrorResponse.Create(
                statusCode: 500,
                message: "Test error",
                details: "Detailed message",
                stackTrace: "at Test.Method()",
                path: "/api/test",
                traceId: "trace-123"
            );

            // Assert
            Assert.False(response.Success);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal("Test error", response.Message);
            Assert.Equal("Detailed message", response.Details);
            Assert.Equal("at Test.Method()", response.StackTrace);
            Assert.Equal("/api/test", response.Path);
            Assert.Equal("trace-123", response.TraceId);
            Assert.True(response.Timestamp <= DateTime.UtcNow);
        }

        [Fact]
        public void ApiErrorResponse_Create_GeneratesTraceIdWhenNull()
        {
            // Arrange & Act
            var response = ApiErrorResponse.Create(500, "Test error");

            // Assert
            Assert.NotNull(response.TraceId);
            Assert.NotEmpty(response.TraceId);
        }

        [Fact]
        public void ApiErrorResponse_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var response = new ApiErrorResponse();

            // Assert
            Assert.False(response.Success);
            Assert.Equal(0, response.StatusCode);
            Assert.Equal(string.Empty, response.Message);
            Assert.Null(response.Details);
            Assert.Null(response.StackTrace);
        }

        #endregion

        #region Middleware Basic Behavior Tests

        [Fact]
        public async Task InvokeAsync_NoException_PassesThroughSuccessfully()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert - response body should be empty (no error written)
            Assert.Equal(0, context.Response.Body.Length);
        }

        [Fact]
        public async Task InvokeAsync_Exception_SetsContentTypeToJson()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new Exception("Test exception"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_Exception_ReturnsValidJsonResponse()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new Exception("Test exception"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.NotNull(errorResponse);
            Assert.False(errorResponse.Success);
            Assert.NotEmpty(errorResponse.TraceId);
        }

        [Fact]
        public async Task InvokeAsync_Exception_IncludesRequestPath()
        {
            // Arrange
            var context = CreateHttpContext();
            context.Request.Path = "/api/customers/123";
            var middleware = CreateMiddleware(_ => throw new Exception("Test exception"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.Equal("/api/customers/123", errorResponse?.Path);
        }

        [Fact]
        public async Task InvokeAsync_Exception_LogsError()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new Exception("Test exception"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Environment-Based Detail Exposure Tests

        [Fact]
        public async Task InvokeAsync_DevelopmentEnvironment_IncludesExceptionDetails()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(
                _ => throw new Exception("Detailed error message"),
                isDevelopment: true);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.NotNull(errorResponse?.Details);
            Assert.Contains("Detailed error message", errorResponse.Details);
        }

        [Fact]
        public async Task InvokeAsync_DevelopmentEnvironment_IncludesStackTrace()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(
                _ => throw new Exception("Test"),
                isDevelopment: true);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.NotNull(errorResponse?.StackTrace);
        }

        [Fact]
        public async Task InvokeAsync_ProductionEnvironment_HidesExceptionDetails()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(
                _ => throw new Exception("Sensitive error details"),
                isDevelopment: false);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.Null(errorResponse?.Details);
        }

        [Fact]
        public async Task InvokeAsync_ProductionEnvironment_HidesStackTrace()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(
                _ => throw new Exception("Test"),
                isDevelopment: false);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.Null(errorResponse?.StackTrace);
        }

        #endregion

        #region Exception Type Mapping Tests

        [Fact]
        public async Task InvokeAsync_ArgumentNullException_Returns400BadRequest()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new ArgumentNullException("paramName"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("required parameter", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvokeAsync_ArgumentException_Returns400BadRequest()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new ArgumentException("Invalid argument"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("Invalid argument", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvokeAsync_KeyNotFoundException_Returns404NotFound()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new KeyNotFoundException("Entity not found"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("not found", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvokeAsync_UnauthorizedAccessException_Returns401Unauthorized()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new UnauthorizedAccessException("Access denied"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("Access denied", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvokeAsync_InvalidOperationException_Returns409Conflict()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Invalid state"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Conflict, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_NotImplementedException_Returns501NotImplemented()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new NotImplementedException("Feature pending"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotImplemented, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("not yet implemented", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvokeAsync_TimeoutException_Returns408RequestTimeout()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new TimeoutException("Operation timed out"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.RequestTimeout, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("timed out", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvokeAsync_OperationCanceledException_Returns400BadRequest()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new OperationCanceledException("Cancelled"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("cancelled", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvokeAsync_GenericException_Returns500InternalServerError()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new Exception("Unexpected error"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            var errorResponse = await GetErrorResponse(context);
            Assert.Contains("unexpected error", errorResponse?.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task InvokeAsync_NestedInnerException_HandlesGracefully()
        {
            // Arrange
            var context = CreateHttpContext();
            var innerException = new ArgumentException("Inner error");
            var outerException = new InvalidOperationException("Outer error", innerException);
            var middleware = CreateMiddleware(_ => throw outerException);

            // Act
            await middleware.InvokeAsync(context);

            // Assert - should handle the outer exception type
            Assert.Equal((int)HttpStatusCode.Conflict, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_EmptyExceptionMessage_ReturnsGenericMessage()
        {
            // Arrange
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new Exception(""));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.NotNull(errorResponse?.Message);
            Assert.NotEmpty(errorResponse.Message);
        }

        [Fact]
        public async Task InvokeAsync_PreservesTraceIdentifier()
        {
            // Arrange
            var context = CreateHttpContext();
            context.TraceIdentifier = "unique-trace-12345";
            var middleware = CreateMiddleware(_ => throw new Exception("Test"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            var errorResponse = await GetErrorResponse(context);
            Assert.Equal("unique-trace-12345", errorResponse?.TraceId);
        }

        #endregion
    }
}
