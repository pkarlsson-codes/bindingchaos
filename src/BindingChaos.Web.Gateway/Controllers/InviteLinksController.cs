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
}
