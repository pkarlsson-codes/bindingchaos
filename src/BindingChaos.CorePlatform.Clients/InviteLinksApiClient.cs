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
public sealed class InviteLinksApiClient(HttpClient httpClient, ILogger<InviteLinksApiClient> logger)
    : BaseApiClient(httpClient, logger), IInviteLinksApiClient
{
    /// <inheritdoc/>
    public Task<InviteLinkCreatedResponse> CreateInviteLinkAsync(CreateInviteLinkRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<CreateInviteLinkRequest, InviteLinkCreatedResponse>("api/identity/invite-links", request, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<InviteLinkViewResponse>> GetMyInviteLinksAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetCollectionAsync<InviteLinkViewResponse>("api/identity/invite-links", cancellationToken).ConfigureAwait(false);
        return result.ToList();
    }

    /// <inheritdoc/>
    public Task RevokeInviteLinkAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/identity/invite-links/{id}", cancellationToken);
    }
}
