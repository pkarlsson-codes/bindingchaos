using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for resolving participant profiles by pseudonym.
/// </summary>
/// <param name="profilesApiClient">The API client for interacting with the profiles service.</param>
[ApiController]
[Route("api/v1/profiles")]
public sealed class ProfilesController(IProfilesApiClient profilesApiClient) : BaseApiController
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
    [AllowAnonymous]
    public async Task<IActionResult> GetProfile(
        [FromRoute] string pseudonym,
        CancellationToken cancellationToken)
    {
        var profile = await profilesApiClient
            .GetProfileAsync(pseudonym, cancellationToken)
            .ConfigureAwait(false);

        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }
}
