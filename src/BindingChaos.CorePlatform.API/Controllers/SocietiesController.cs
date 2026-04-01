using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.IdentityProfile.Application.Commands;
using BindingChaos.IdentityProfile.Application.Queries;
using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.Societies.Application.Commands;
using BindingChaos.Societies.Application.Queries;
using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Domain.SocialContracts;
using BindingChaos.Societies.Domain.Societies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing societies.
/// </summary>
/// <param name="messageBus">The message bus for dispatching commands and queries.</param>
/// <param name="pseudonymService">The pseudonym lookup service for resolving participant identities.</param>
[ApiController]
[Route("api/societies")]
public sealed class SocietiesController(IMessageBus messageBus, IPseudonymLookupService pseudonymService) : BaseApiController
{
    /// <summary>
    /// Creates a new society together with its initial social contract.
    /// </summary>
    /// <param name="request">The society creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created society.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("createSociety")]
    public async Task<IActionResult> CreateSociety([FromBody] CreateSocietyRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        GeographicArea? geoBounds = request.GeographicBounds is not null
            ? new GeographicArea(
                request.GeographicBounds.North,
                request.GeographicBounds.South,
                request.GeographicBounds.East,
                request.GeographicBounds.West)
            : null;

        Coordinates? center = request.Center is not null
            ? new Coordinates(request.Center.Latitude, request.Center.Longitude)
            : null;

        var command = new CreateSociety(
            participantId,
            request.Name,
            request.Description,
            request.Tags,
            request.RatificationThreshold,
            request.ReviewWindowHours,
            request.AllowVeto,
            request.RequiredVerificationWeight,
            geoBounds,
            center);

        var societyId = await messageBus.InvokeAsync<SocietyId>(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetSociety), new { societyId = societyId.Value }, societyId.Value);
    }

    /// <summary>
    /// Gets a paginated list of societies with optional tag and geographic filtering.
    /// </summary>
    /// <param name="request">The pagination and filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of societies.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SocietyListItemResponse>>), 200)]
    [EndpointName("getSocieties")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSocieties(
        [FromQuery] PaginationQuerySpec<SocietiesQueryFilter> request,
        CancellationToken cancellationToken)
    {
        var result = await messageBus
            .InvokeAsync<PaginatedResponse<SocietyListItemView>>(new GetSocieties(request), cancellationToken)
            .ConfigureAwait(false);

        return Ok(result.MapItems(MapToListItemResponse));
    }

    /// <summary>
    /// Gets a single society by ID.
    /// </summary>
    /// <param name="societyId">The society ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The society, or 404 if not found.</returns>
    [HttpGet("{societyId}")]
    [ProducesResponseType(typeof(ApiResponse<SocietyResponse>), 200)]
    [EndpointName("getSociety")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSociety([FromRoute] string societyId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);

        var id = SocietyId.Create(societyId);
        var view = await messageBus.InvokeAsync<SocietyView?>(new GetSociety(id), cancellationToken).ConfigureAwait(false);

        if (view is null)
        {
            return NotFound($"Society with ID {societyId} not found.");
        }

        var contractId = await messageBus.InvokeAsync<string?>(new GetCurrentSocialContractId(id), cancellationToken).ConfigureAwait(false);

        var createdByPseudonym = await pseudonymService.GetPseudonymAsync(view.CreatedBy, cancellationToken).ConfigureAwait(false) ?? view.CreatedBy;

        return Ok(MapToSocietyResponse(view, createdByPseudonym, contractId));
    }

    /// <summary>
    /// Gets a paginated list of active members for a society.
    /// </summary>
    /// <param name="societyId">The society ID.</param>
    /// <param name="request">The pagination parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of members.</returns>
    [HttpGet("{societyId}/members")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SocietyMemberResponse>>), 200)]
    [EndpointName("getSocietyMembers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSocietyMembers(
        [FromRoute] string societyId,
        [FromQuery] PaginationQuerySpec<EmptyFilter> request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);

        var id = SocietyId.Create(societyId);
        var query = new GetSocietyMembers(id, request);
        var result = await messageBus
            .InvokeAsync<PaginatedResponse<SocietyMemberView>>(query, cancellationToken)
            .ConfigureAwait(false);

        var pseudonyms = await pseudonymService.GetPseudonymsAsync(result.Items.Select(m => m.ParticipantId), cancellationToken).ConfigureAwait(false);

        return Ok(result.MapItems(m => MapToMemberResponse(m, pseudonyms)));
    }

    /// <summary>
    /// Joins a society. The body must specify the social contract ID to agree to.
    /// </summary>
    /// <param name="societyId">The society to join.</param>
    /// <param name="request">The join request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The new membership ID, or an error response.</returns>
    [HttpPost("{societyId}/memberships")]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("joinSociety")]
    public async Task<IActionResult> JoinSociety(
        [FromRoute] string societyId,
        [FromBody] JoinSocietyRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new JoinSociety(
            SocietyId.Create(societyId),
            participantId,
            SocialContractId.Create(request.SocialContractId));

        var membershipId = await messageBus
            .InvokeAsync<MembershipId>(command, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(
            nameof(GetSocietyMembers),
            new { societyId },
            membershipId.Value);
    }

    /// <summary>
    /// Gets the IDs of all societies the authenticated participant is an active member of.
    /// Returns an empty array for unauthenticated requests.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of society ID strings.</returns>
    [HttpGet("memberships/me")]
    [ProducesResponseType(typeof(ApiResponse<string[]>), 200)]
    [EndpointName("getMySocietyIds")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMySocietyIds(CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Ok(Array.Empty<string>());
        }

        var query = new GetParticipantSocietyIds(participantId);
        var ids = await messageBus.InvokeAsync<string[]>(query, cancellationToken).ConfigureAwait(false);

        return Ok(ids);
    }

    /// <summary>
    /// Removes the current participant's active membership from a society.
    /// </summary>
    /// <param name="societyId">The society to leave.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpDelete("{societyId}/memberships/me")]
    [ProducesResponseType(204)]
    [EndpointName("leaveSociety")]
    public async Task<IActionResult> LeaveSociety([FromRoute] string societyId, CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new LeaveSociety(SocietyId.Create(societyId), participantId);
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Returns all invite links created by the authenticated participant for the specified society.
    /// </summary>
    /// <param name="societyId">The society whose invite links are being retrieved.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The participant's invite links for the society, or 401/403 if unauthorized.</returns>
    [HttpGet("{societyId}/invite-links/mine")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SocietyInviteLinkViewResponse>>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [EndpointName("getMySocietyInviteLinks")]
    public async Task<IActionResult> GetMySocietyInviteLinks(
        [FromRoute] string societyId,
        CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var memberSocietyIds = await messageBus
            .InvokeAsync<string[]>(new GetParticipantSocietyIds(participantId), cancellationToken)
            .ConfigureAwait(false);

        if (!memberSocietyIds.Contains(societyId))
        {
            return Forbid();
        }

        var query = new GetMySocietyInviteLinks(participantId.Value, societyId);
        var result = await messageBus
            .InvokeAsync<IReadOnlyList<SocietyInviteLinkView>>(query, cancellationToken)
            .ConfigureAwait(false);

        var response = result.Select(v => new SocietyInviteLinkViewResponse(v.Id, v.Token, v.SocietyId, v.Note, v.IsRevoked, v.CreatedAt)).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Creates an invite link for the specified society. The caller must be an active member.
    /// </summary>
    /// <param name="societyId">The society to create an invite link for.</param>
    /// <param name="request">The invite link creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created invite link, or 401/403 if unauthorized.</returns>
    [HttpPost("{societyId}/invite-links")]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [EndpointName("createSocietyInviteLink")]
    public async Task<IActionResult> CreateSocietyInviteLink(
        [FromRoute] string societyId,
        [FromBody] CreateSocietyInviteLinkRequest request,
        CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var memberSocietyIds = await messageBus
            .InvokeAsync<string[]>(new GetParticipantSocietyIds(participantId), cancellationToken)
            .ConfigureAwait(false);

        if (!memberSocietyIds.Contains(societyId))
        {
            return Forbid();
        }

        var command = new CreateSocietyInviteLink(participantId.Value, societyId, request.Note);
        var id = await messageBus
            .InvokeAsync<Guid>(command, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(nameof(GetMySocietyInviteLinks), new { societyId }, id);
    }

    private static SocietyResponse MapToSocietyResponse(SocietyView view, string createdByPseudonym, string? currentSocialContractId = null)
    {
        return new SocietyResponse(
            view.Id,
            view.Name,
            view.Description,
            createdByPseudonym,
            view.CreatedAt,
            [.. view.Tags],
            view.HasGeographicBounds,
            view.GeographicBoundsJson,
            view.CenterJson,
            [.. view.Relationships.Select(r => new SocietyRelationshipResponse(r.TargetSocietyId, r.RelationshipType.ToString()))],
            view.ActiveMemberCount,
            currentSocialContractId);
    }

    private static SocietyListItemResponse MapToListItemResponse(SocietyListItemView view)
    {
        return new SocietyListItemResponse(
            view.Id,
            view.Name,
            view.Description,
            view.CreatedAt,
            [.. view.Tags],
            view.HasGeographicBounds,
            view.ActiveMemberCount);
    }

    private static SocietyMemberResponse MapToMemberResponse(SocietyMemberView view, IReadOnlyDictionary<string, string> pseudonyms)
    {
        if (!pseudonyms.TryGetValue(view.ParticipantId, out var pseudonym))
        {
            throw new InvalidOperationException($"No pseudonym found for participant {view.ParticipantId}.");
        }

        return new SocietyMemberResponse(
            view.Id,
            pseudonym,
            view.SocialContractId,
            view.JoinedAt);
    }
}
