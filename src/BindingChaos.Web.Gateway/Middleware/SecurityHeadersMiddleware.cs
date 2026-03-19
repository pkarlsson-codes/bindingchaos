namespace BindingChaos.Web.Gateway.Middleware;

/// <summary>
/// Middleware helpers for adding common security headers.
/// </summary>
public static class SecurityHeadersExtensions
{
    /// <summary>
    /// Adds security headers to every response.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers.XFrameOptions = "DENY";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers.XXSSProtection = "0";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            await next().ConfigureAwait(false);
        });
        return app;
    }
}
