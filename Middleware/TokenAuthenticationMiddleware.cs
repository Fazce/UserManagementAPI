using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    // Middleware to authenticate requests using Bearer tokens
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string AUTH_HEADER = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
        // For demo: a hardcoded valid token. In production, use a secure validation method.
        private const string VALID_TOKEN = "mysecrettoken123";

        // Constructor to initialize the middleware with the next delegate
        public TokenAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow unauthenticated access to Swagger endpoints
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico")))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.ContainsKey(AUTH_HEADER))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Missing Authorization header." });
                return;
            }

            var authHeader = context.Request.Headers[AUTH_HEADER].ToString();
            if (!authHeader.StartsWith(BEARER_PREFIX))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid Authorization header format." });
                return;
            }

            var token = authHeader.Substring(BEARER_PREFIX.Length);
            if (token != VALID_TOKEN)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid or expired token." });
                return;
            }

            await _next(context);
        }
    }

    // Extension method to add the middleware to the HTTP request pipeline
    public static class TokenAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthenticationMiddleware>();
        }
    }
}
