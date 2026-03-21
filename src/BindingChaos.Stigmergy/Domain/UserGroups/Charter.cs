using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Value object representing the full charter of a <see cref="UserGroup"/>, encompassing approval, membership, and shunning rules.
/// </summary>
public sealed class Charter : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Charter"/> class.
    /// </summary>
    /// <param name="approvalRules">The rules governing approval-based decisions.</param>
    /// <param name="membershipRules">The rules governing membership.</param>
    /// <param name="shunningRules">The rules governing shunning behaviour.</param>
    public Charter(ContentionRules approvalRules, MembershipRules membershipRules, ShunningRules shunningRules)
    {
        ApprovalRules = approvalRules;
        MembershipRules = membershipRules;
        ShunningRules = shunningRules;
    }

    /// <summary>
    /// Gets the rules governing approval-based decisions in this group.
    /// </summary>
    public ContentionRules ApprovalRules { get; }

    /// <summary>
    /// Gets the rules governing membership in this group.
    /// </summary>
    public MembershipRules MembershipRules { get; }

    /// <summary>
    /// Gets the rules governing shunning behaviour in this group.
    /// </summary>
    public ShunningRules ShunningRules { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ApprovalRules;
        yield return MembershipRules;
        yield return ShunningRules;
    }
}
