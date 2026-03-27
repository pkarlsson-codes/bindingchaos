using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Gateway controller for invite link operations.
/// </summary>
/// <param name="inviteLinksApiClient">Client for interacting with the Invite Links API.</param>
[ApiController]
[Route("api/v1/invite-links")]
public sealed class TrustTrustInviteLinksController(ITrustTrustInviteLinksApiClient inviteLinksApiClient) : BaseApiController
{
    /// <summary>
    /// Gets all invite links for the authenticated participant, sorted by creation date descending.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All invite links (active and revoked) for the participant.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrustInviteLinkViewResponse>>), 200)]
    [EndpointName("getMyTrustTrustInviteLinks")]
    public async Task<IActionResult> GetMyTrustTrustInviteLinks(CancellationToken cancellationToken)
    {
        var result = await inviteLinksApiClient.GetMyTrustTrustInviteLinksAsync(cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Creates an invite link for the authenticated participant.
    /// </summary>
    /// <param name="request">The invite link creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created invite link details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TrustInviteLinkCreatedResponse>), 201)]
    [EndpointName("createTrustInviteLink")]
    public async Task<IActionResult> CreateTrustInviteLink([FromBody] CreateTrustInviteLinkRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await inviteLinksApiClient.CreateTrustInviteLinkAsync(request, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(CreateTrustInviteLink), result);
    }

    /// <summary>
    /// Revokes an invite link owned by the authenticated participant.
    /// </summary>
    /// <param name="id">The ID of the invite link to revoke.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>204 No Content on success; 403 if the caller does not own the link; 404 if not found.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [EndpointName("revokeTrustInviteLink")]
    public async Task<IActionResult> RevokeTrustInviteLink([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await inviteLinksApiClient.RevokeTrustInviteLinkAsync(id, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}
