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
    /// <param name="contentionRules">The rules governing contention-based decisions.</param>
    /// <param name="membershipRules">The rules governing membership.</param>
    /// <param name="shunningRules">The rules governing shunning behaviour.</param>
    public Charter(ContentionRules contentionRules, MembershipRules membershipRules, ShunningRules shunningRules)
    {
        ContentionRules = contentionRules;
        MembershipRules = membershipRules;
        ShunningRules = shunningRules;
    }

    /// <summary>
    /// Gets the rules governing contention-based decisions in this group.
    /// </summary>
    public ContentionRules ContentionRules { get; }

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
        yield return ContentionRules;
        yield return MembershipRules;
        yield return ShunningRules;
    }
}
