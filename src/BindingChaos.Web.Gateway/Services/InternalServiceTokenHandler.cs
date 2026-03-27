using BindingChaos.Web.Gateway.Configuration;

namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Delegating handler that attaches a short-lived internal service JWT to outbound requests.
/// </summary>
public sealed class InternalServiceTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IInternalJwtService _jwtService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalServiceTokenHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor used to read the current session cookie.</param>
    /// <param name="jwtService">Service used to mint internal service tokens.</param>
    public InternalServiceTokenHandler(IHttpContextAccessor httpContextAccessor, IInternalJwtService jwtService)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[GatewayDefaults.Cookies.SessionCookie];
        var token = _jwtService.GenerateServiceToken(sessionId);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
