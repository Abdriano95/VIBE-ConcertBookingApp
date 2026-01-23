using System.Text.Json.Serialization;

namespace DMA_AU24_LAB2_Group4.Data.DTO
{
    /// <summary>
    /// Standardized API error response for consistent error handling across all endpoints.
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; } = false;

        /// <summary>
        /// HTTP status code of the error.
        /// </summary>
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Detailed error information (only included in development environment).
        /// </summary>
        [JsonPropertyName("details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Details { get; set; }

        /// <summary>
        /// Stack trace for debugging (only included in development environment).
        /// </summary>
        [JsonPropertyName("stackTrace")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StackTrace { get; set; }

        /// <summary>
        /// Unique trace ID for correlating logs.
        /// </summary>
        [JsonPropertyName("traceId")]
        public string TraceId { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the error occurred.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The API path that caused the error.
        /// </summary>
        [JsonPropertyName("path")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Path { get; set; }

        /// <summary>
        /// Creates a standardized error response.
        /// </summary>
        public static ApiErrorResponse Create(int statusCode, string message, string? details = null, string? stackTrace = null, string? path = null, string? traceId = null)
        {
            return new ApiErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Details = details,
                StackTrace = stackTrace,
                Path = path,
                TraceId = traceId ?? Guid.NewGuid().ToString()
            };
        }
    }
}
