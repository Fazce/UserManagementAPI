using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    // Middleware to log incoming requests and outgoing responses
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Invoke method to process HTTP requests and responses
        public async Task InvokeAsync(HttpContext context)
        {
            // Log incoming request
            _logger.LogInformation("Incoming Request: {method} {path}", context.Request.Method, context.Request.Path);

            // Copy original response body to capture status code
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Log outgoing response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            _logger.LogInformation("Outgoing Response: {statusCode}", context.Response.StatusCode);
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    // Extension method to add the middleware to the HTTP request pipeline
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
