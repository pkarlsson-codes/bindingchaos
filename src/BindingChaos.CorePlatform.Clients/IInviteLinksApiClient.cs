using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for interacting with the Invite Links API.
/// </summary>
public interface IInviteLinksApiClient
{
    /// <summary>
    /// Creates an invite link for the authenticated participant.
    /// </summary>
    /// <param name="request">The invite link creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created invite link details.</returns>
    Task<InviteLinkCreatedResponse> CreateInviteLinkAsync(CreateInviteLinkRequest request, CancellationToken cancellationToken = default);
}
