using System.Security.Claims;

namespace TodoListApp.WebApi.Middleware;

/// <summary>
/// Middleware for API key Bearer authentication.
/// </summary>
public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private const string AuthorizationHeader = "Authorization";
    private const string BearerPrefix = "Bearer ";

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public ApiKeyAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="configuration">The configuration.</param>
    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (ShouldSkipAuth(context))
        {
            await _next(context);
            return;
        }

        var apiKey = configuration["Authentication:ApiKey"] ?? string.Empty;
        var authHeader = context.Request.Headers[AuthorizationHeader].FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) &&
            authHeader.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader[BearerPrefix.Length..].Trim();
            if (token == apiKey)
            {
                var identity = new ClaimsIdentity("ApiKey");
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "api-user"));
                context.User = new ClaimsPrincipal(identity);
                await _next(context);
                return;
            }
        }

        context.Response.StatusCode = 401;
    }

    private static bool ShouldSkipAuth(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        return path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("/health", StringComparison.OrdinalIgnoreCase);
    }
}
