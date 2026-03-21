using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Value object describing the rules that govern approval-based membership decisions within a <see cref="UserGroup"/>.
/// </summary>
public sealed class ApprovalRules : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApprovalRules"/> class.
    /// </summary>
    /// <param name="requiredApprovals">The minimum number of approvals required.</param>
    /// <param name="votingPeriod">The duration of the voting window.</param>
    public ApprovalRules(int requiredApprovals, TimeSpan votingPeriod)
    {
        RequiredApprovals = requiredApprovals;
        VotingPeriod = votingPeriod;
    }

    /// <summary>
    /// Gets the minimum number of approvals required to accept a membership request.
    /// </summary>
    public int RequiredApprovals { get; }

    /// <summary>
    /// Gets the duration of the voting window for membership approval decisions.
    /// </summary>
    public TimeSpan VotingPeriod { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RequiredApprovals;
        yield return VotingPeriod;
    }
}
