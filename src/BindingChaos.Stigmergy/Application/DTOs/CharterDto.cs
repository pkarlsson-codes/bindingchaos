namespace BindingChaos.Stigmergy.Application.DTOs;

/// <summary>
/// User Group Charter.
/// </summary>
/// <param name="ContestationRules">The contestation rules.</param>
/// <param name="MembershipRules">The membership rules.</param>
/// <param name="ShunningRules">The shunning rules.</param>
public sealed record CharterDto(
    ContestationRulesDto ContestationRules,
    MembershipRulesDto MembershipRules,
    ShunningRulesDto ShunningRules);

/// <summary>
/// Contestation rules.
/// </summary>
/// <param name="ResolutionWindow">The resolution window.</param>
/// <param name="RejectionThreshold">The rejection threshold, represented as a decimal between 0 and 1.</param>
public sealed record ContestationRulesDto(TimeSpan ResolutionWindow, decimal RejectionThreshold);

/// <summary>
/// Approval settings for membership or other approval-based processes.
/// </summary>
/// <param name="ApprovalThreshold">The threshold for approval decisions, represented as a decimal between 0 and 1.</param>
/// <param name="VetoEnabled">Indicates whether veto power is enabled.</param>
public sealed record ApprovalSettingsDto(decimal ApprovalThreshold, bool VetoEnabled);

/// <summary>
/// Membership rules.
/// </summary>
/// <param name="JoinPolicy">The join policy.</param>
/// <param name="ApprovalSettings">The approval settings, if applicable based on the join policy.</param>
/// <param name="MaxMembers">The maximum number of members allowed in the group, if any.</param>
/// <param name="EntryRequirements">A description of any specific requirements for joining the group, such as qualifications or criteria.</param>
/// <param name="MemberListPublic">Indicates whether the member list is public or private.</param>
public sealed record MembershipRulesDto(
    JoinPolicyDto JoinPolicy,
    ApprovalSettingsDto? ApprovalSettings,
    int? MaxMembers,
    string? EntryRequirements,
    bool MemberListPublic);

/// <summary>
/// Shunning rules.
/// </summary>
/// <param name="ApprovalThreshold">The threshold for shunning decisions, represented as a decimal between 0 and 1.</param>
public sealed record ShunningRulesDto(decimal ApprovalThreshold);

/// <summary>
/// Specifies the policies that determine how users can join a group.
/// </summary>
public enum JoinPolicyDto
{
    /// <summary>
    /// Open.
    /// </summary>
    Open,

    /// <summary>
    /// Invite only.
    /// </summary>
    InviteOnly,

    /// <summary>
    /// Approval process.
    /// </summary>
    Approval,
}