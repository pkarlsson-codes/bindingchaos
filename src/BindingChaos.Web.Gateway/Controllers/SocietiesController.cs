using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Web.Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using DomainRequests = BindingChaos.CorePlatform.Contracts.Requests;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing societies in the web gateway.
/// </summary>
/// <param name="societiesApiClient">Client for interacting with the Societies API.</param>
[ApiController]
[Route("api/v1/societies")]
public sealed class SocietiesController(ISocietiesApiClient societiesApiClient) : BaseApiController
{
    /// <summary>
    /// Gets a paginated list of societies with optional filtering.
    /// </summary>
    /// <param name="query">Pagination and filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>Paginated list of societies.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<SocietiesFeedViewModel>), 200)]
    [EndpointName("getSocieties")]
    public async Task<OkObjectResult> GetSocieties(
        [FromQuery] PaginationQuerySpec<SocietiesQueryFilter> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var result = await societiesApiClient
            .GetSocietiesAsync(query.Normalize(), cancellationToken);

        var response = new SocietiesFeedViewModel
        {
            Societies = result,
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets detailed information about a specific society.
    /// </summary>
    /// <param name="societyId">The unique identifier of the society.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>Detailed society information.</returns>
    [HttpGet("{societyId}")]
    [ProducesResponseType(typeof(ApiResponse<SocietyDetailViewModel>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getSociety")]
    public async Task<IActionResult> GetSocietyDetails(
        string societyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);

        var result = await societiesApiClient
            .GetSocietyAsync(societyId, cancellationToken);

        var viewModel = new SocietyDetailViewModel
        {
            Society = result,
        };

        return Ok(viewModel);
    }

    /// <summary>
    /// Gets the active members of a specific society.
    /// </summary>
    /// <param name="societyId">The unique identifier of the society.</param>
    /// <param name="query">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>Paginated list of society members.</returns>
    [HttpGet("{societyId}/members")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SocietyMemberResponse>>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getSocietyMembers")]
    public async Task<IActionResult> GetSocietyMembers(
        string societyId,
        [FromQuery] PaginationQuerySpec<EmptyFilter> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        ArgumentNullException.ThrowIfNull(query);

        var result = await societiesApiClient
        .GetSocietyMembersAsync(
            societyId,
            query.Normalize(),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new society.
    /// </summary>
    /// <param name="request">The society creation request.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created society ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("createSociety")]
    public async Task<IActionResult> CreateSociety(
        [FromBody] CreateSocietyRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var bounds = request.GeographicBounds is not null
            ? new DomainRequests.GeographicBoundsRequest(
                request.GeographicBounds.North,
                request.GeographicBounds.South,
                request.GeographicBounds.East,
                request.GeographicBounds.West)
            : null;
        var center = request.Center is not null
            ? new DomainRequests.CoordinatesRequest(
                request.Center.Latitude,
                request.Center.Longitude)
            : null;

        var domainRequest = new DomainRequests.CreateSocietyRequest(
            request.Name,
            request.Description,
            request.Tags,
            request.RatificationThreshold,
            request.ReviewWindowHours,
            request.AllowVeto,
            request.RequiredVerificationWeight,
            bounds,
            center);

        var societyId = await societiesApiClient.CreateSocietyAsync(
            domainRequest,
            cancellationToken);

        return CreatedAtAction(nameof(GetSocietyDetails), new { societyId }, societyId);
    }

    /// <summary>
    /// Joins a society on behalf of the authenticated participant.
    /// </summary>
    /// <param name="societyId">The unique identifier of the society to join.</param>
    /// <param name="request">The join request containing the social contract ID.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created membership ID.</returns>
    [HttpPost("{societyId}/memberships")]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("joinSociety")]
    public async Task<IActionResult> JoinSociety(
        string societyId,
        [FromBody] DomainRequests.JoinSocietyRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        ArgumentNullException.ThrowIfNull(request);

        var membershipId = await societiesApiClient
            .JoinSocietyAsync(societyId, request, cancellationToken);

        return CreatedAtAction(
            nameof(GetSocietyMembers),
            new { societyId },
            membershipId);
    }

    /// <summary>
    /// Gets the IDs of all societies the authenticated participant is an active member of.
    /// Returns an empty array for unauthenticated requests.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>An array of society ID strings.</returns>
    [HttpGet("memberships/me")]
    [ProducesResponseType(typeof(ApiResponse<string[]>), 200)]
    [EndpointName("getMySocietyIds")]
    public async Task<IActionResult> GetMySocietyIds(
        CancellationToken cancellationToken)
    {
        var ids = await societiesApiClient
            .GetMySocietyIdsAsync(cancellationToken);
        return Ok(ids);
    }

    /// <summary>
    /// Removes the authenticated participant's membership from a society.
    /// </summary>
    /// <param name="societyId">The unique identifier of the society to leave.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{societyId}/memberships/me")]
    [ProducesResponseType(204)]
    [EndpointName("leaveSociety")]
    public async Task<IActionResult> LeaveSociety(
        string societyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);

        await societiesApiClient
            .LeaveSocietyAsync(societyId, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Gets all invite links created by the authenticated participant for the specified society.
    /// </summary>
    /// <param name="societyId">The unique identifier of the society.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The participant's invite links for the society.</returns>
    [HttpGet("{societyId}/invite-links/mine")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SocietyInviteLinkViewResponse>>), 200)]
    [EndpointName("getMySocietyInviteLinks")]
    public async Task<IActionResult> GetMySocietyInviteLinks(
        string societyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);

        var result = await societiesApiClient
            .GetMySocietyInviteLinksAsync(societyId, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Creates an invite link for the specified society. The caller must be an active member.
    /// Membership is verified downstream in CorePlatform.API.
    /// </summary>
    /// <param name="societyId">The unique identifier of the society.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The ID of the created invite link.</returns>
    [HttpPost("{societyId}/invite-links")]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("createSocietyInviteLink")]
    public async Task<IActionResult> CreateSocietyInviteLink(
        string societyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);

        var id = await societiesApiClient
            .CreateSocietyInviteLinkAsync(societyId, cancellationToken);

        return CreatedAtAction(nameof(GetMySocietyInviteLinks), new { societyId }, id);
    }
}
