using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Invite Links API client.
/// </summary>
/// <param name="httpClient">The HTTP client to use for API requests.</param>
/// <param name="logger">The logger for this client.</param>
public sealed class TrustInviteLinksApiClient(HttpClient httpClient, ILogger<TrustInviteLinksApiClient> logger)
    : BaseApiClient(httpClient, logger), ITrustInviteLinksApiClient
{
    /// <inheritdoc/>
    public Task<string> CreateTrustInviteLinkAsync(CreateTrustInviteLinkRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<CreateTrustInviteLinkRequest, string>("api/identity/invite-links", request, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TrustInviteLinksResponse> GetMyTrustInviteLinksAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<TrustInviteLinksResponse>("api/identity/invite-links", cancellationToken);
    }

    /// <inheritdoc/>
    public Task RevokeTrustInviteLinkAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/identity/invite-links/{id}", cancellationToken);
    }

    /// <inheritdoc/>
    public Task<ResolvedInviteLinkResponse> ResolveTrustInviteLinkAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        return GetAsync<ResolvedInviteLinkResponse>($"api/identity/invite-links/resolve?token={Uri.EscapeDataString(token)}", cancellationToken);
    }
}
