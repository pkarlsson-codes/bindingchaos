using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.DTOs;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Forms a new user group to govern the specified commons.</summary>
/// <param name="CommonsId">The ID of the commons this group will govern.</param>
/// <param name="FounderId">The ID of the participant forming the group.</param>
/// <param name="Name">The name of the new user group.</param>
/// <param name="Charter">The charter defining the rules and policies of the group.</param>
public sealed record FormUserGroup(CommonsId CommonsId, ParticipantId FounderId, string Name, CharterDto Charter);

/// <summary>Handles the <see cref="FormUserGroup"/> command.</summary>
public static class FormUserGroupHandler
{
    /// <summary>Handles the <see cref="FormUserGroup"/> command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="userGroupRepository">The document session used to persist the new user group.</param>
    /// <param name="commonsRepository">The repository to check for the existence of the specified commons.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the formed user group.</returns>
    public static async Task<UserGroupId> Handle(
        FormUserGroup command,
        IUserGroupRepository userGroupRepository,
        ICommonsRepository commonsRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!await commonsRepository.ExistsByIdAsync(command.CommonsId, cancellationToken).ConfigureAwait(false))
        {
            throw new AggregateNotFoundException(typeof(Commons), command.CommonsId);
        }

        var charter = CreateCharterHelper.CreateCharter(command.Charter);
        var userGroup = UserGroup.Form(command.FounderId, command.CommonsId, command.Name, charter);
        userGroupRepository.Stage(userGroup);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return userGroup.Id;
    }
}

/// <summary>Shared helper for building a <see cref="Charter"/> from a <see cref="CharterDto"/>.</summary>
public static class CreateCharterHelper
{
    /// <summary>Builds a <see cref="Charter"/> domain object from a <see cref="CharterDto"/>.</summary>
    /// <param name="charterDto">The charter data transfer object.</param>
    /// <returns>The constructed <see cref="Charter"/>.</returns>
    public static Charter CreateCharter(CharterDto charterDto)
    {
        var approvalSettings = charterDto.MembershipRules.ApprovalSettings is not null
            ? new MembershipApprovalRules(
                charterDto.MembershipRules.ApprovalSettings.ApprovalThreshold,
                charterDto.MembershipRules.ApprovalSettings.VetoEnabled)
            : null;
        return new Charter(
            new ContentionRules(charterDto.ContestationRules.RejectionThreshold, charterDto.ContestationRules.ResolutionWindow),
            new MembershipRules(
                charterDto.MembershipRules.JoinPolicy switch
                {
                    JoinPolicyDto.Open => JoinPolicy.Open,
                    JoinPolicyDto.InviteOnly => JoinPolicy.InviteOnly,
                    JoinPolicyDto.Approval => JoinPolicy.Approval,
                    _ => throw new ArgumentOutOfRangeException(nameof(charterDto), "Invalid join policy.")
                },
                charterDto.MembershipRules.MemberListPublic,
                charterDto.MembershipRules.MaxMembers,
                charterDto.MembershipRules.EntryRequirements,
                approvalSettings),
            new ShunningRules(charterDto.ShunningRules.ApprovalThreshold));
    }
}
