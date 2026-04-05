using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.IdentityProfile.Application.Queries;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Endpoints for resolving participant profiles by pseudonym.
/// </summary>
/// <param name="messageBus">The message bus for dispatching queries.</param>
[ApiController]
[Route("api/profiles")]
public sealed class ProfilesController(IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Returns the public profile of a participant identified by their pseudonym.
    /// </summary>
    /// <param name="pseudonym">The participant's globally unique pseudonym.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The participant's profile, or 404 if not found.</returns>
    [HttpGet("{pseudonym}")]
    [ProducesResponseType(typeof(ApiResponse<ParticipantProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("getParticipantProfile")]
    public async Task<IActionResult> GetProfile([FromRoute] string pseudonym, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pseudonym);

        var result = await messageBus
            .InvokeAsync<BindingChaos.IdentityProfile.Application.ReadModels.ParticipantView?>(
                new GetParticipantByPseudonym(pseudonym),
                cancellationToken)
            .ConfigureAwait(false);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(new ParticipantProfileResponse(result.UserId, result.Pseudonym, result.JoinedAt));
    }
}
