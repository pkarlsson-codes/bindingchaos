using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for interacting with the Invite Links API.
/// </summary>
public interface ITrustTrustInviteLinksApiClient
{
    /// <summary>
    /// Creates an invite link for the authenticated participant.
    /// </summary>
    /// <param name="request">The invite link creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created invite link details.</returns>
    Task<TrustInviteLinkCreatedResponse> CreateTrustInviteLinkAsync(CreateTrustInviteLinkRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all invite links for the authenticated participant, sorted by creation date descending.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All invite links (active and revoked) for the participant.</returns>
    Task<IReadOnlyList<TrustInviteLinkViewResponse>> GetMyTrustTrustInviteLinksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an invite link owned by the authenticated participant.
    /// </summary>
    /// <param name="id">The ID of the invite link to revoke.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RevokeTrustInviteLinkAsync(Guid id, CancellationToken cancellationToken = default);
}
