using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Value object describing the rules that govern shunning behaviour within a <see cref="UserGroup"/>.
/// This is a placeholder for future shunning policy properties.
/// </summary>
public sealed class ShunningRules : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShunningRules"/> class.
    /// </summary>
    /// <param name="approvalThreshold">The threshold for shunning decisions, represented as a decimal between 0 and 1.</param>
    public ShunningRules(decimal approvalThreshold)
    {
        if (approvalThreshold < 0 || approvalThreshold > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(approvalThreshold), "Approval threshold must be between 0 and 1.");
        }

        ApprovalThreshold = approvalThreshold;
    }

    /// <summary>
    /// Gets the threshold for shunning decisions, represented as a decimal between 0 and 1.
    /// </summary>
    public decimal ApprovalThreshold { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ApprovalThreshold;
    }
}
