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
    public Task<TrustInviteLinkCreatedResponse> CreateTrustInviteLinkAsync(CreateTrustInviteLinkRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<CreateTrustInviteLinkRequest, TrustInviteLinkCreatedResponse>("api/identity/invite-links", request, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TrustInviteLinkViewResponse>> GetMyTrustInviteLinksAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetCollectionAsync<TrustInviteLinkViewResponse>("api/identity/invite-links", cancellationToken).ConfigureAwait(false);
        return result.ToList();
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
