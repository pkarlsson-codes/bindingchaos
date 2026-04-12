namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>Assembled read model for the user group detail view.</summary>
public sealed record UserGroupDetailView(
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
    UserGroupCharterView Charter);

/// <summary>Charter view for a user group.</summary>
public sealed record UserGroupCharterView(
    UserGroupMembershipRulesView MembershipRules,
    UserGroupContestationRulesView ContestationRules,
    UserGroupShunningRulesView ShunningRules);

/// <summary>Membership rules view for a user group charter.</summary>
public sealed record UserGroupMembershipRulesView(
    string JoinPolicy,
    int? MaxMembers,
    string? EntryRequirements,
    bool MemberListPublic,
    UserGroupApprovalSettingsView? ApprovalSettings);

/// <summary>Approval settings view for a user group membership policy.</summary>
public sealed record UserGroupApprovalSettingsView(
    double ApprovalThreshold,
    bool VetoEnabled);

/// <summary>Contestation rules view for a user group charter.</summary>
public sealed record UserGroupContestationRulesView(
    string ResolutionWindow,
    double RejectionThreshold);

/// <summary>Shunning rules view for a user group charter.</summary>
public sealed record UserGroupShunningRulesView(double ApprovalThreshold);
