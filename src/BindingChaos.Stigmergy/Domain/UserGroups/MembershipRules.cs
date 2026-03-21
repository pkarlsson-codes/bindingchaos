using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Value object describing the rules that govern membership within a <see cref="UserGroup"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MembershipRules"/> class.
/// </remarks>
/// <param name="joinPolicy">The policy that determines how participants may join.</param>
/// <param name="memberListPublic">A value indicating whether the member list is public.</param>
/// <param name="maxMembers">The maximum number of members allowed in the group.</param>
/// <param name="entryRequirements">The entry requirements for the group, if any.</param>
/// <param name="approvalRules">The rules that govern approval-based membership decisions.</param>
public sealed class MembershipRules(JoinPolicy joinPolicy, bool memberListPublic, int? maxMembers = null, string? entryRequirements = null, MembershipApprovalRules? approvalRules = null) : ValueObject
{

    /// <summary>
    /// Gets the policy that determines how participants may join this group.
    /// </summary>
    public JoinPolicy JoinPolicy { get; } = joinPolicy;

    /// <summary>
    /// Gets a value indicating whether the member list of this group is public.
    /// </summary>
    public bool MemberListPublic { get; } = memberListPublic;

    /// <summary>
    /// Gets the maximum number of members allowed in this group, or null if there is no limit.
    /// </summary>
    public int? MaxMembers { get; } = maxMembers;

    /// <summary>
    /// Gets the rules that govern approval-based membership decisions for this group, or null if approval is not required.
    /// </summary>
    public MembershipApprovalRules? ApprovalRules { get; } = approvalRules;

    /// <summary>
    /// Gets the entry requirements for this group, or null if there are no specific requirements.
    /// </summary>
    public string? EntryRequirements { get; } = entryRequirements;

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return JoinPolicy;
        yield return MemberListPublic;
        yield return MaxMembers ?? 0;
        yield return ApprovalRules ?? new MembershipApprovalRules(0, false);
        yield return EntryRequirements ?? string.Empty;
    }
}

/// <summary>
/// Value object describing the rules that govern approval-based membership decisions within a <see cref="UserGroup"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MembershipApprovalRules"/> class.
/// </remarks>
/// <param name="approvalThreshold">The threshold for approving a membership request.</param>
/// <param name="vetoEnabled">A value indicating whether veto power is enabled for this group.</param>
public sealed class MembershipApprovalRules(decimal approvalThreshold, bool vetoEnabled) : ValueObject
{

    /// <summary>
    /// Gets the threshold for approving a membership request.
    /// </summary>
    public decimal ApprovalThreshold { get; } = approvalThreshold;

    /// <summary>
    /// Gets a value indicating whether veto power is enabled for this group.
    /// </summary>
    public bool VetoEnabled { get; } = vetoEnabled;

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ApprovalThreshold;
        yield return VetoEnabled;
    }
}