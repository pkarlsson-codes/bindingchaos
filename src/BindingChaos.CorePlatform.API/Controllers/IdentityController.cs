using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.IdentityProfile.Application.Commands;
using BindingChaos.IdentityProfile.Application.Queries;
using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.SharedKernel.Domain;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Endpoints for identity mapping.
/// </summary>
/// <param name="service">Identity profile service.</param>
/// <param name="messageBus">The message bus for dispatching commands.</param>
[ApiController]
[Route("api/identity")]
public sealed class IdentityController(IIdentityProfileService service, IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Links (or retrieves) the internal user id for a given external identity.
    /// </summary>
    /// <param name="request">Link request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Link response containing the internal user id.</returns>
    [HttpPost("users/link")]
    [ProducesResponseType(typeof(ApiResponse<LinkResponse>), 200)]
    [EndpointName("linkUserIdentity")]
    public async Task<IActionResult> Link([FromBody] IdentityController.LinkRequest request, CancellationToken cancellationToken)
    {
        var userId = await service.LinkOrGetUserIdAsync(request.Provider, request.Subject, cancellationToken).ConfigureAwait(false);
        return Ok(new LinkResponse { UserId = userId });
    }

    /// <summary>
    /// Gets identity info for a user.
    /// </summary>
    /// <param name="id">Internal user id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>User identity view.</returns>
    [HttpGet("users/{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserView>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getUserProfile")]
    public async Task<IActionResult> GetUser([FromRoute] string id, CancellationToken cancellationToken)
    {
        var map = await service.GetIdentityMapAsync(id, cancellationToken).ConfigureAwait(false);
        if (map is null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        return Ok(new UserView { UserId = id });
    }

    /// <summary>
    /// Gets all invite links for the authenticated participant, sorted by creation date descending.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All invite links (active and revoked) for the participant.</returns>
    [HttpGet("invite-links")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<InviteLinkViewResponse>>), 200)]
    [EndpointName("getMyInviteLinks")]
    public async Task<IActionResult> GetMyInviteLinks(CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var query = new GetMyInviteLinks(participantId.Value);
        var result = await messageBus.InvokeAsync<IReadOnlyList<InviteLinkView>>(query, cancellationToken).ConfigureAwait(false);

        var response = result.Select(v => new InviteLinkViewResponse(v.Id, v.Token, v.Note, v.IsRevoked, v.CreatedAt)).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Creates an invite link for the authenticated participant.
    /// </summary>
    /// <param name="request">The invite link creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created invite link details.</returns>
    [HttpPost("invite-links")]
    [ProducesResponseType(typeof(ApiResponse<InviteLinkCreatedResponse>), 201)]
    [EndpointName("createInviteLink")]
    public async Task<IActionResult> CreateInviteLink([FromBody] CreateInviteLinkRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new CreateInviteLink(participantId.Value, request.Note);
        var result = await messageBus.InvokeAsync<InviteLinkCreatedView>(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(CreateInviteLink), result);
    }

    /// <summary>
    /// Revokes an invite link owned by the authenticated participant.
    /// </summary>
    /// <param name="id">The ID of the invite link to revoke.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>204 No Content on success; 403 if the caller does not own the link; 404 if not found.</returns>
    [HttpDelete("invite-links/{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [EndpointName("revokeInviteLink")]
    public async Task<IActionResult> RevokeInviteLink([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new RevokeInviteLink(id, participantId.Value);
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Request to link an external identity to an internal user id.
    /// </summary>
    public sealed class LinkRequest
    {
        /// <summary>
        /// The external identity provider (e.g., "google", "github").
        /// </summary>
        required public string Provider { get; init; }

        /// <summary>
        /// The subject identifier for the external identity (e.g., email, username).
        /// </summary>
        required public string Subject { get; init; }
    }

    /// <summary>
    /// Response containing the internal user id after linking an external identity.
    /// </summary>
    private sealed class LinkResponse
    {
        /// <summary>
        /// The internal user id associated with the external identity.
        /// </summary>
        required public string UserId { get; init; }
    }

    /// <summary>
    /// View model for user identity information.
    /// </summary>
    private sealed class UserView
    {
        /// <summary>
        /// The internal user id.
        /// </summary>
        required public string UserId { get; init; }
    }
}
