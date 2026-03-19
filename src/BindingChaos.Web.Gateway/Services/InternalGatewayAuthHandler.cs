using System.Net.Http.Headers;

namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Delegating handler that attaches a short-lived internal JWT to calls to CorePlatform.
/// </summary>
public sealed partial class InternalGatewayAuthHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IInternalJwtService _jwtService;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<InternalGatewayAuthHandler> _logger;

    public InternalGatewayAuthHandler(
        IHttpContextAccessor httpContextAccessor,
        IInternalJwtService jwtService,
        ITokenStore tokenStore,
        ILogger<InternalGatewayAuthHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        string? participantId = null;
        bool? personhoodVerified = null;
        string? trustLevel = null;
        string? sessionId = null;

        if (httpContext != null)
        {
            var requestPath = httpContext.Request.Path.Value ?? string.Empty;

            participantId = httpContext.User.FindFirst("userId")?.Value
                             ?? httpContext.User.FindFirst("participant_id")?.Value;

            sessionId = httpContext.Request.Cookies["bc_session"];

            if (string.IsNullOrWhiteSpace(participantId) && !string.IsNullOrWhiteSpace(sessionId))
            {
                var stored = await _tokenStore.TryGetTokensAsync(sessionId, cancellationToken).ConfigureAwait(false);
                if (stored.HasValue)
                {
                    participantId = stored.Value.userId;
                    Logs.ResolvedFromTokenStore(_logger, participantId);
                }
            }

            if (string.IsNullOrWhiteSpace(participantId))
            {
                Logs.AnonymousRequest(_logger, requestPath);
            }
            else
            {
                Logs.ForwardingWithParticipant(_logger, participantId, requestPath);
            }
        }

        var token = _jwtService.GenerateToken(participantId, personhoodVerified, trustLevel, sessionId);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Resolved participantId {ParticipantId} from token store for session (claims principal had no userId claim)")]
        internal static partial void ResolvedFromTokenStore(ILogger logger, string participantId);

        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "No participantId resolved from claims or token store; downstream request will be treated as anonymous. Path: {Path}")]
        internal static partial void AnonymousRequest(ILogger logger, string path);

        [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Forwarding request with participantId {ParticipantId}. Path: {Path}")]
        internal static partial void ForwardingWithParticipant(ILogger logger, string participantId, string path);
    }
}
