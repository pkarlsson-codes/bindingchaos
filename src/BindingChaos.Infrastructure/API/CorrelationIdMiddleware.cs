using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BindingChaos.Infrastructure.API;

/// <summary>
/// Middleware to capture or generate a correlation ID from request headers and store it in HttpContext.Items.
/// </summary>
public class CorrelationIdMiddleware
{
    /// <summary>
    /// The HTTP header name used to carry the correlation ID.
    /// </summary>
    internal const string CorrelationIdHeader = "X-Correlation-Id";

    /// <summary>
    /// The key used to store the correlation ID in <see cref="HttpContext.Items"/>.
    /// </summary>
    internal const string CorrelationIdItemKey = "CorrelationId";

    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="next"/> is null.</exception>
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    /// <summary>
    /// Invokes the middleware to capture or generate a correlation ID.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request. This context is used to read the correlation ID from headers or generate a new one if not present.</param>
    /// <returns>A task that represents the asynchronous operation. The task will complete when the next middleware in the pipeline has been invoked.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        string? correlationId = null;
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue))
        {
            correlationId = headerValue.FirstOrDefault();
        }

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items[CorrelationIdItemKey] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        await _next(context).ConfigureAwait(false);
    }
}

/// <summary>
/// Extension methods for the CorrelationIdMiddleware.
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    /// <summary>
    /// Registers the CorrelationIdMiddleware in the ASP.NET Core request pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to register the middleware with.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> with the CorrelationIdMiddleware registered.</returns>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }

    /// <summary>
    /// Gets the correlation ID from HttpContext.Items, if present.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/> to read from.</param>
    /// <returns>The correlation ID if present; otherwise, <see langword="null"/>.</returns>
    public static string? GetCorrelationId(this HttpContext context)
    {
        if (context == null)
        {
            return null;
        }

        if (context.Items.TryGetValue(CorrelationIdMiddleware.CorrelationIdItemKey, out var value) && value is string correlationId)
        {
            return correlationId;
        }

        return null;
    }

    /// <summary>
    /// Sets the correlation ID header on an outgoing <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="request">The outgoing HTTP request message.</param>
    /// <param name="correlationId">The correlation ID to set on the request.</param>
    public static void SetCorrelationId(this HttpRequestMessage request, string? correlationId)
    {
        if (request == null || string.IsNullOrWhiteSpace(correlationId))
        {
            return;
        }

        request.Headers.Remove(CorrelationIdMiddleware.CorrelationIdHeader);
        request.Headers.Add(CorrelationIdMiddleware.CorrelationIdHeader, correlationId);
    }
}
