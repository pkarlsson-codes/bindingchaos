using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Value object describing the rules that govern membership within a <see cref="UserGroup"/>.
/// </summary>
public sealed class MembershipRules : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MembershipRules"/> class.
    /// </summary>
    /// <param name="joinPolicy">The policy that determines how participants may join.</param>
    public MembershipRules(JoinPolicy joinPolicy)
    {
        JoinPolicy = joinPolicy;
    }

    /// <summary>
    /// Gets the policy that determines how participants may join this group.
    /// </summary>
    public JoinPolicy JoinPolicy { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return JoinPolicy;
    }
}
