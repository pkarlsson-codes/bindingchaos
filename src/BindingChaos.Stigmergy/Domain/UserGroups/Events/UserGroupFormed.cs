using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.UserGroups.Events;

/// <summary>Raised when a new <see cref="UserGroup"/> is formed to govern a commons.</summary>
/// <param name="UserGroupId">The identifier of the formed user group.</param>
/// <param name="CommonsId">The identifier of the commons this group governs.</param>
/// <param name="FounderId">The identifier of the participant who founded the group.</param>
/// <param name="Name">The name of the user group.</param>
/// <param name="Charter">The charter that governs the user group.</param>
internal sealed record UserGroupFormed(
    string UserGroupId,
    string CommonsId,
    string FounderId,
    string Name,
    CharterRecord Charter)
    : DomainEvent(UserGroupId);

/// <summary>Snapshot of a <see cref="Charter"/> captured in the <see cref="UserGroupFormed"/> event.</summary>
public sealed record CharterRecord(
    ContentionRulesRecord ContentionRules,
    MembershipRulesRecord MembershipRules,
    ShunningRulesRecord ShunningRules);

/// <summary>Snapshot of <see cref="ContentionRules"/> captured in <see cref="UserGroupFormed"/>.</summary>
public sealed record ContentionRulesRecord(decimal RejectionThreshold, TimeSpan ResolutionWindow);

/// <summary>Snapshot of <see cref="MembershipRules"/> captured in <see cref="UserGroupFormed"/>.</summary>
public sealed record MembershipRulesRecord(
    int JoinPolicy,
    bool MemberListPublic,
    int? MaxMembers,
    string? EntryRequirements,
    MembershipApprovalRulesRecord? ApprovalRules);

/// <summary>Snapshot of <see cref="MembershipApprovalRules"/> captured in <see cref="UserGroupFormed"/>.</summary>
public sealed record MembershipApprovalRulesRecord(decimal ApprovalThreshold, bool VetoEnabled);

/// <summary>Snapshot of <see cref="ShunningRules"/> captured in <see cref="UserGroupFormed"/>.</summary>
public sealed record ShunningRulesRecord(decimal ApprovalThreshold);
