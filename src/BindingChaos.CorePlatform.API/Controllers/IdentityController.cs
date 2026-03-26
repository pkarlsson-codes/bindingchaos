using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Endpoints for identity mapping.
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
