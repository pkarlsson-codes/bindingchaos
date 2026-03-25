namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for forming a new user group.
/// </summary>
/// <param name="CommonsId">The ID of the commons this user group will govern.</param>
/// <param name="Name">The name of the user group.</param>
/// <param name="Charter">The charter defining the rules and policies of the group.</param>
public sealed record FormUserGroupRequest(string CommonsId, string Name, UserGroupCharterDto Charter);

/// <summary>
/// Charter data transfer object.
/// </summary>
/// <param name="ContestationRules">The contestation rules.</param>
/// <param name="MembershipRules">The membership rules.</param>
/// <param name="ShunningRules">The shunning rules.</param>
public sealed record UserGroupCharterDto(
    UserGroupContestationRulesDto ContestationRules,
    UserGroupMembershipRulesDto MembershipRules,
    UserGroupShunningRulesDto ShunningRules);

/// <summary>
/// Contestation rules.
/// </summary>
/// <param name="ResolutionWindow">The resolution window.</param>
/// <param name="RejectionThreshold">The rejection threshold, represented as a decimal between 0 and 1.</param>
public sealed record UserGroupContestationRulesDto(TimeSpan ResolutionWindow, decimal RejectionThreshold);

/// <summary>
/// Membership rules.
/// </summary>
/// <param name="JoinPolicy">The join policy.</param>
/// <param name="ApprovalSettings">The approval settings, if applicable based on the join policy.</param>
/// <param name="MaxMembers">The maximum number of members allowed in the group, if any.</param>
/// <param name="EntryRequirements">A description of any specific requirements for joining the group.</param>
/// <param name="MemberListPublic">Indicates whether the member list is public or private.</param>
public sealed record UserGroupMembershipRulesDto(
    UserGroupJoinPolicyDto JoinPolicy,
    UserGroupApprovalSettingsDto? ApprovalSettings,
    int? MaxMembers,
    string? EntryRequirements,
    bool MemberListPublic);

/// <summary>
/// Shunning rules.
/// </summary>
/// <param name="ApprovalThreshold">The threshold for shunning decisions, represented as a decimal between 0 and 1.</param>
public sealed record UserGroupShunningRulesDto(decimal ApprovalThreshold);

/// <summary>
/// Approval settings for membership.
/// </summary>
/// <param name="ApprovalThreshold">The threshold for approval decisions, represented as a decimal between 0 and 1.</param>
/// <param name="VetoEnabled">Indicates whether veto power is enabled.</param>
public sealed record UserGroupApprovalSettingsDto(decimal ApprovalThreshold, bool VetoEnabled);

/// <summary>
/// Specifies the policies that determine how users can join a group.
/// </summary>
public enum UserGroupJoinPolicyDto
{
    /// <summary>Open to anyone.</summary>
    Open,

    /// <summary>Invite only.</summary>
    InviteOnly,

    /// <summary>Requires approval.</summary>
    Approval,
}
