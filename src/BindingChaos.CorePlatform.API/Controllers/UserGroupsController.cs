using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.DTOs;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using CharterDto = BindingChaos.Stigmergy.Application.DTOs.CharterDto;
using JoinPolicyDto = BindingChaos.Stigmergy.Application.DTOs.JoinPolicyDto;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing user groups.
/// </summary>
/// <param name="messageBus">The message bus instance used for publishing events or messages.</param>
/// <param name="pseudonymLookupService">A service used to resolve user pseudonyms from participant identifiers.</param>
[ApiController]
[Route("api/usergroups")]
public sealed class UserGroupsController(
    IMessageBus messageBus,
    IPseudonymLookupService pseudonymLookupService)
    : BaseApiController
{
    /// <summary>
    /// Forms a new user group to govern a commons.
    /// </summary>
    /// <param name="request">The formation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the formed user group.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("formUserGroup")]
    public async Task<IActionResult> FormUserGroup([FromBody] FormUserGroupRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var charter = MapCharter(request.Charter);
        var command = new FormUserGroup(
            CommonsId.Create(request.CommonsId),
            participantId,
            request.Name,
            request.Philosophy,
            charter);
        var userGroupId = await messageBus.InvokeAsync<UserGroupId>(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetUserGroupDetail), new { id = userGroupId.Value }, userGroupId.Value);
    }

    /// <summary>
    /// Retrieves all user groups governing the specified commons.
    /// </summary>
    /// <param name="commonsId">The ID of the commons to filter by.</param>
    /// <param name="queryRequest">Pagination and sorting parameters from the querystring.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of user groups.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserGroupListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getUserGroupsForCommons")]
    public async Task<IActionResult> GetUserGroupsForCommons(
        [FromQuery] string commonsId,
        [FromQuery] PaginationQuerySpec queryRequest,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(commonsId))
        {
            return BadRequest("commonsId is required.");
        }

        var userGroups = await messageBus
            .InvokeAsync<PaginatedResponse<UserGroupListItemView>>(
                new GetUserGroupsForCommons(CommonsId.Create(commonsId), queryRequest),
                cancellationToken)
            .ConfigureAwait(false);

        var pseudonyms = await pseudonymLookupService.GetPseudonymsAsync(
            userGroups.Items.Select(g => g.FounderId),
            cancellationToken);

        var response = userGroups.MapItems(
            g => new UserGroupListItemResponse(
                g.Id,
                g.CommonsId,
                g.Name,
                g.Philosophy,
                pseudonyms.GetValueOrDefault(g.FounderId, "Anonymous"),
                g.FormedAt,
                g.MemberCount,
                g.JoinPolicy));

        return Ok(response);
    }

    /// <summary>
    /// Retrieves all user groups that the authenticated participant is a member of.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of user groups the current participant belongs to.</returns>
    [HttpGet("mine")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupListItemResponse[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("getMyUserGroups")]
    public async Task<IActionResult> GetMyUserGroups(CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var userGroups = await messageBus
            .InvokeAsync<UserGroupListItemView[]>(
                new GetMyUserGroups(participantId),
                cancellationToken)
            .ConfigureAwait(false);

        var pseudonyms = await pseudonymLookupService.GetPseudonymsAsync(
            userGroups.Select(g => g.FounderId),
            cancellationToken);

        var response = userGroups
            .Select(g => new UserGroupListItemResponse(
                g.Id,
                g.CommonsId,
                g.Name,
                g.Philosophy,
                pseudonyms.GetValueOrDefault(g.FounderId, "Anonymous"),
                g.FormedAt,
                g.MemberCount,
                g.JoinPolicy))
            .ToArray();

        return Ok(response);
    }

    /// <summary>
    /// Retrieves the detail of a single user group by ID.
    /// </summary>
    /// <param name="id">The user group ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user group detail, or 404 if not found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("getUserGroupDetail")]
    public async Task<IActionResult> GetUserGroupDetail(string id, CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        var callerId = participantId == ParticipantId.Anonymous ? null : participantId;

        var view = await messageBus
            .InvokeAsync<UserGroupDetailView?>(
                new GetUserGroupDetail(UserGroupId.Create(id), callerId),
                cancellationToken)
            .ConfigureAwait(false);

        if (view is null)
        {
            return NotFound();
        }

        var approvalSettings = view.Charter.MembershipRules.ApprovalSettings is null
            ? null
            : new UserGroupApprovalSettingsResponse(
                view.Charter.MembershipRules.ApprovalSettings.ApprovalThreshold,
                view.Charter.MembershipRules.ApprovalSettings.VetoEnabled);

        var response = new UserGroupDetailResponse(
            view.Id,
            view.CommonsId,
            view.CommonsName,
            view.Name,
            view.Philosophy,
            view.FoundedByPseudonym,
            view.FormedAt,
            view.MemberCount,
            view.JoinPolicy,
            view.IsMember,
            new UserGroupCharterResponse(
                new UserGroupMembershipRulesResponse(
                    view.Charter.MembershipRules.JoinPolicy,
                    view.Charter.MembershipRules.MaxMembers,
                    view.Charter.MembershipRules.EntryRequirements,
                    view.Charter.MembershipRules.MemberListPublic,
                    approvalSettings),
                new UserGroupContestationRulesResponse(
                    view.Charter.ContestationRules.ResolutionWindow,
                    view.Charter.ContestationRules.RejectionThreshold),
                new UserGroupShunningRulesResponse(
                    view.Charter.ShunningRules.ApprovalThreshold)));

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a paginated list of members for the specified user group.
    /// </summary>
    /// <param name="id">The user group ID.</param>
    /// <param name="queryRequest">Pagination and sorting parameters from the querystring.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of member pseudonyms, or 404 if the group does not exist, or 403 if the member list is private and the caller is not a member.</returns>
    [HttpGet("{id}/members")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserGroupMemberResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointName("getUserGroupMembers")]
    public async Task<IActionResult> GetUserGroupMembers(
        string id,
        [FromQuery] PaginationQuerySpec queryRequest,
        CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        var callerId = participantId == ParticipantId.Anonymous ? null : participantId;

        try
        {
            var result = await messageBus
                .InvokeAsync<PaginatedResponse<UserGroupMemberView>?>(
                    new GetUserGroupMembers(UserGroupId.Create(id), callerId, queryRequest),
                    cancellationToken)
                .ConfigureAwait(false);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result.MapItems(v => new UserGroupMemberResponse(v.Pseudonym)));
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
    }

    /// <summary>
    /// Retrieves all user groups that the specified participant is a member of.
    /// </summary>
    /// <param name="participantId">The participant ID to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of user groups the participant belongs to.</returns>
    [HttpGet("for-participant")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupListItemResponse[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getUserGroupsForParticipant")]
    public async Task<IActionResult> GetUserGroupsForParticipant(
        [FromQuery] string participantId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(participantId))
        {
            return BadRequest("participantId is required.");
        }

        var userGroups = await messageBus
            .InvokeAsync<UserGroupListItemView[]>(
                new GetMyUserGroups(new ParticipantId(participantId)),
                cancellationToken)
            .ConfigureAwait(false);

        var pseudonyms = await pseudonymLookupService.GetPseudonymsAsync(
            userGroups.Select(g => g.FounderId),
            cancellationToken);

        var response = userGroups
            .Select(g => new UserGroupListItemResponse(
                g.Id,
                g.CommonsId,
                g.Name,
                g.Philosophy,
                pseudonyms.GetValueOrDefault(g.FounderId, "Anonymous"),
                g.FormedAt,
                g.MemberCount,
                g.JoinPolicy))
            .ToArray();

        return Ok(response);
    }

    private static CharterDto MapCharter(UserGroupCharterDto dto)
    {
        var approvalSettings = dto.MembershipRules.ApprovalSettings is not null
            ? new ApprovalSettingsDto(
                dto.MembershipRules.ApprovalSettings.ApprovalThreshold,
                dto.MembershipRules.ApprovalSettings.VetoEnabled)
            : null;

        return new CharterDto(
            new ContestationRulesDto(dto.ContestationRules.ResolutionWindow, dto.ContestationRules.RejectionThreshold),
            new MembershipRulesDto(
                dto.MembershipRules.JoinPolicy switch
                {
                    UserGroupJoinPolicyDto.Open => JoinPolicyDto.Open,
                    UserGroupJoinPolicyDto.InviteOnly => JoinPolicyDto.InviteOnly,
                    UserGroupJoinPolicyDto.Approval => JoinPolicyDto.Approval,
                    _ => throw new ArgumentOutOfRangeException(nameof(dto), "Invalid join policy.")
                },
                approvalSettings,
                dto.MembershipRules.MaxMembers,
                dto.MembershipRules.EntryRequirements,
                dto.MembershipRules.MemberListPublic),
            new ShunningRulesDto(dto.ShunningRules.ApprovalThreshold));
    }
}
