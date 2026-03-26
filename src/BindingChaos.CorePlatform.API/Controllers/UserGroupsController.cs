using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.DTOs;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using CharterDto = BindingChaos.Stigmergy.Application.DTOs.CharterDto;
using JoinPolicyDto = BindingChaos.Stigmergy.Application.DTOs.JoinPolicyDto;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing user groups.
/// </summary>
/// <param name="messageBus">The message bus instance used for publishing events or messages.</param>
[ApiController]
[Route("api/usergroups")]
public sealed class UserGroupsController(IMessageBus messageBus) : BaseApiController
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

        return Created($"api/usergroups/{userGroupId.Value}", userGroupId.Value);
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
