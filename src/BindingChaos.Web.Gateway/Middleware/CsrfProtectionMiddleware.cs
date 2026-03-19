namespace BindingChaos.Web.Gateway.Middleware;

/// <summary>
/// Middleware helpers for CSRF validation based on cookie + header token and allowed origin.
/// </summary>
public static class CsrfProtectionExtensions
{
    /// <summary>
    /// Applies CSRF validation to state-changing requests.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseCsrfProtection(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(context.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(context.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                await next().ConfigureAwait(false);
                return;
            }

            if (context.Request.Path.StartsWithSegments("/auth/logout", StringComparison.OrdinalIgnoreCase))
            {
                await next().ConfigureAwait(false);
                return;
            }

            var allowedOrigins = context.RequestServices.GetRequiredService<IConfiguration>()
                .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            var origin = context.Request.Headers["Origin"].ToString();
            var referer = context.Request.Headers["Referer"].ToString();
            bool originOk = !string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
            bool refererOk = !string.IsNullOrEmpty(referer) && allowedOrigins.Any(o => referer.StartsWith(o, StringComparison.OrdinalIgnoreCase));
            if (!originOk && !refererOk)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid origin" }).ConfigureAwait(false);
                return;
            }

            var csrfCookie = context.Request.Cookies["bc_csrf"];
            context.Request.Headers.TryGetValue("X-CSRF-Token", out var csrfHeaderRaw);
            var csrfHeader = Uri.UnescapeDataString(csrfHeaderRaw.ToString());
            if (string.IsNullOrWhiteSpace(csrfCookie) || string.IsNullOrWhiteSpace(csrfHeader) || csrfCookie != csrfHeader)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "CSRF validation failed" }).ConfigureAwait(false);
                return;
            }

            await next().ConfigureAwait(false);
        });
        return app;
    }
}
