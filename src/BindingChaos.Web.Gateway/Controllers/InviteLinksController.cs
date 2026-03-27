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
public sealed class InviteLinksController(IInviteLinksApiClient inviteLinksApiClient) : BaseApiController
{
    /// <summary>
    /// Gets all invite links for the authenticated participant, sorted by creation date descending.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All invite links (active and revoked) for the participant.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<InviteLinkViewResponse>>), 200)]
    [EndpointName("getMyInviteLinks")]
    public async Task<IActionResult> GetMyInviteLinks(CancellationToken cancellationToken)
    {
        var result = await inviteLinksApiClient.GetMyInviteLinksAsync(cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Creates an invite link for the authenticated participant.
    /// </summary>
    /// <param name="request">The invite link creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created invite link details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InviteLinkCreatedResponse>), 201)]
    [EndpointName("createInviteLink")]
    public async Task<IActionResult> CreateInviteLink([FromBody] CreateInviteLinkRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await inviteLinksApiClient.CreateInviteLinkAsync(request, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(CreateInviteLink), result);
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
    [EndpointName("revokeInviteLink")]
    public async Task<IActionResult> RevokeInviteLink([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await inviteLinksApiClient.RevokeInviteLinkAsync(id, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}
