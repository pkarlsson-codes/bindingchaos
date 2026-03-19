using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Endpoints for identity mapping and user trust.
/// </summary>
/// <param name="service">Identity profile service.</param>
[ApiController]
[Route("api/identity")]
public sealed class IdentityController(IIdentityProfileService service) : BaseApiController
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
    /// Gets identity and trust info for a user.
    /// </summary>
    /// <param name="id">Internal user id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>User identity/trust view.</returns>
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

        var trust = await service.GetOrCreateTrustAsync(id, cancellationToken).ConfigureAwait(false);
        return Ok(new UserView
        {
            UserId = id,
            PersonhoodVerified = trust.PersonhoodVerified,
            TrustLevel = trust.TrustLevel,
        });
    }

    /// <summary>
    /// Sets a user's personhood verified flag.
    /// </summary>
    /// <param name="id">Internal user id.</param>
    /// <param name="request">Personhood request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Updated user identity/trust view.</returns>
    [HttpPost("users/{id}/personhood")]
    [ProducesResponseType(typeof(ApiResponse<UserView>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("setUserPersonhood")]
    public async Task<IActionResult> SetPersonhood([FromRoute] string id, [FromBody] PersonhoodRequest request, CancellationToken cancellationToken)
    {
        var map = await service.GetIdentityMapAsync(id, cancellationToken).ConfigureAwait(false);
        if (map is null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        var trust = await service.SetPersonhoodAsync(id, request.Verified, cancellationToken).ConfigureAwait(false);
        return Ok(new UserView
        {
            UserId = id,
            PersonhoodVerified = trust.PersonhoodVerified,
            TrustLevel = trust.TrustLevel,
        });
    }

    // Private request/response types (kept private to avoid public XML docs and ordering issues)

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
    /// Request to set a user's personhood verification status.
    /// </summary>
    public sealed class PersonhoodRequest
    {
        /// <summary>
        /// Indicates whether the user's personhood has been verified.
        /// </summary>
        public bool Verified { get; init; }
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
    /// View model for user identity and trust information.
    /// </summary>
    private sealed class UserView
    {
        /// <summary>
        /// The internal user id.
        /// </summary>
        required public string UserId { get; init; }

        /// <summary>
        /// Indicates whether the user's personhood has been verified.
        /// </summary>
        public bool PersonhoodVerified { get; init; }

        /// <summary>
        /// The trust level of the user (e.g., "unknown", "trusted", "untrusted").
        /// </summary>
        required public string TrustLevel { get; init; }
    }
}
