namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents detailed information about a user group.
/// </summary>
/// <param name="Id">The unique identifier of the user group.</param>
/// <param name="CommonsId">The identifier of the commons this group governs.</param>
/// <param name="CommonsName">The name of the commons this group governs.</param>
/// <param name="Name">The name of the user group.</param>
/// <param name="Philosophy">The philosophy of the user group, if set.</param>
/// <param name="FoundedByPseudonym">The pseudonym of the participant who founded the group.</param>
/// <param name="FormedAt">The timestamp when the group was formed.</param>
/// <param name="MemberCount">The current number of members in the group.</param>
/// <param name="JoinPolicy">The join policy of the group.</param>
/// <param name="IsMember">Whether the requesting participant is a member of the group.</param>
/// <param name="Charter">The charter governing the group.</param>
public sealed record UserGroupDetailResponse(
    string Id,
    string CommonsId,
    string CommonsName,
    string Name,
    string? Philosophy,
    string FoundedByPseudonym,
    DateTimeOffset FormedAt,
    int MemberCount,
    string JoinPolicy,
    bool IsMember,
    UserGroupCharterResponse Charter);

/// <summary>
/// Represents the charter of a user group.
/// </summary>
/// <param name="MembershipRules">The rules governing membership.</param>
/// <param name="ContestationRules">The rules governing contestation.</param>
/// <param name="ShunningRules">The rules governing shunning.</param>
public sealed record UserGroupCharterResponse(
    UserGroupMembershipRulesResponse MembershipRules,
    UserGroupContestationRulesResponse ContestationRules,
    UserGroupShunningRulesResponse ShunningRules);

/// <summary>
/// Represents the membership rules of a user group charter.
/// </summary>
/// <param name="JoinPolicy">The policy controlling how participants may join.</param>
/// <param name="MaxMembers">The maximum number of members allowed, if set.</param>
/// <param name="EntryRequirements">The entry requirements for joining, if any.</param>
/// <param name="MemberListPublic">Whether the member list is publicly visible.</param>
/// <param name="ApprovalSettings">The approval settings, if approval is required.</param>
public sealed record UserGroupMembershipRulesResponse(
    string JoinPolicy,
    int? MaxMembers,
    string? EntryRequirements,
    bool MemberListPublic,
    UserGroupApprovalSettingsResponse? ApprovalSettings);

/// <summary>
/// Represents the approval settings for a user group membership policy.
/// </summary>
/// <param name="ApprovalThreshold">The fraction of approvals required for a join request to succeed.</param>
/// <param name="VetoEnabled">Whether any member may veto a join request.</param>
public sealed record UserGroupApprovalSettingsResponse(
    double ApprovalThreshold,
    bool VetoEnabled);

/// <summary>
/// Represents the contestation rules of a user group charter.
/// </summary>
/// <param name="ResolutionWindow">The duration allowed for resolving a contestation.</param>
/// <param name="RejectionThreshold">The fraction of rejections required to block a contested action.</param>
public sealed record UserGroupContestationRulesResponse(
    string ResolutionWindow,
    double RejectionThreshold);

/// <summary>
/// Represents the shunning rules of a user group charter.
/// </summary>
/// <param name="ApprovalThreshold">The fraction of member votes required to shun a participant.</param>
public sealed record UserGroupShunningRulesResponse(
    double ApprovalThreshold);
