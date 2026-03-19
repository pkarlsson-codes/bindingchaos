using Microsoft.AspNetCore.Http;

namespace BindingChaos.Infrastructure.API;

/// <summary>
/// DelegatingHandler that appends the correlation ID header to outgoing HTTP requests if available in the current HttpContext.
/// </summary>
public class CorrelationIdHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> to access the current HttpContext.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpContextAccessor"/> is null.</exception>
    public CorrelationIdHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Sends the HTTP request and appends the correlation ID header if available in the HttpContext.
    /// </summary>
    /// <param name="request">The HTTP request message to send. The correlation ID will be set on this request if available in the HttpContext.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the send operation that produces the HTTP response message.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();
        request.SetCorrelationId(correlationId);
        return base.SendAsync(request, cancellationToken);
    }
}
