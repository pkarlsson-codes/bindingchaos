using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.UserGroups.Events;

/// <summary>
/// Domain event representing the creation of a new <see cref="UserGroup"/>. This event is raised when a user group is first created, capturing the initial state of the group including its founder, name, and charter.
/// </summary>
/// <param name="UserGroupId">The identifier of the newly created user group.</param>
/// <param name="FounderId">The identifier of the participant who founded the group.</param>
/// <param name="Name">The name of the user group.</param>
/// <param name="Charter">The charter that governs the user group.</param>
internal sealed record UserGroupCreated(
    string UserGroupId,
    string FounderId,
    string Name,
    CharterRecord Charter)
    : DomainEvent(UserGroupId);

/// <summary>
/// Snapshot of the charter properties at the time of user group creation, used to capture the initial state of the group's governing rules in the <see cref="UserGroupCreated"/> event.
/// </summary>
/// <param name="ContentionRules"></param>
/// <param name="MembershipRules"></param>
/// <param name="ShunningRules"></param>
public sealed record CharterRecord(
    ContentionRulesRecord ContentionRules,
    MembershipRulesRecord MembershipRules,
    ShunningRulesRecord ShunningRules);

/// <summary>
/// Snapshot of the user group's charter properties at the time of creation, used in the <see cref="UserGroupCreated"/> event to capture the initial state of the group's governing rules.
/// </summary>
/// <param name="RejectionThreshold"></param>
/// <param name="ResolutionWindow"></param>
public sealed record ContentionRulesRecord(decimal RejectionThreshold, TimeSpan ResolutionWindow);

/// <summary>
/// Membership rules.
/// </summary>
/// <param name="JoinPolicy"></param>
/// <param name="MemberListPublic"></param>
/// <param name="MaxMembers"></param>
/// <param name="EntryRequirements"></param>
/// <param name="ApprovalRules"></param>
public sealed record MembershipRulesRecord(
    int JoinPolicy,
    bool MemberListPublic,
    int? MaxMembers,
    string? EntryRequirements,
    MembershipApprovalRulesRecord? ApprovalRules);

/// <summary>
/// Membership approval rules.
/// </summary>
/// <param name="ApprovalThreshold"></param>
/// <param name="VetoEnabled"></param>
public sealed record MembershipApprovalRulesRecord(decimal ApprovalThreshold, bool VetoEnabled);

/// <summary>
/// Shunning rules.
/// </summary>
/// <param name="ApprovalThreshold"></param>
public sealed record ShunningRulesRecord(decimal ApprovalThreshold);
