using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.DTOs;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to create a new user group with specified parameters and charter.
/// </summary>
/// <param name="FounderId">The ID of the user who is creating the group.</param>
/// <param name="Name">The name of the new user group.</param>
/// <param name="Charter">The charter defining the rules and policies of the group.</param>
public sealed record CreateUserGroup(string FounderId, string Name, CharterDto Charter);

/// <summary>
/// Handler for the <see cref="CreateUserGroup"/> command, responsible for processing the command and creating a new user group based on the provided parameters and charter.
/// </summary>
public static class CreateUserGroupHandler
{
    /// <summary>
    /// Handles the creation of a new user group by processing the provided command, constructing the appropriate domain entities, and persisting them using the provided document session.
    /// </summary>
    /// <param name="command">The command containing the details for the new user group.</param>
    /// <param name="documentSession">The document session used to persist the new user group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(CreateUserGroup command, IDocumentSession documentSession, CancellationToken cancellationToken)
    {
        var charter = CreateCharter(command.Charter);
        var userGroup = UserGroup.Create(ParticipantId.Create(command.FounderId), command.Name, charter);
        documentSession.Store(userGroup);
        await documentSession.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a <see cref="Charter"/> domain entity from the provided <see cref="CharterDto"/> data transfer object, mapping the properties accordingly and performing any necessary validation or transformation.
    /// </summary>
    /// <param name="charterDto">The data transfer object containing the charter details.</param>
    /// <returns>The created <see cref="Charter"/> domain entity.</returns>
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