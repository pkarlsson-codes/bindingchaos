using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Value object describing the rules that govern approval-based membership decisions within a <see cref="UserGroup"/>.
/// </summary>
public sealed class ContentionRules : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentionRules"/> class.
    /// </summary>
    /// <param name="rejectionThreshold">The threshold for rejecting a membership request.</param>
    /// <param name="resolutionWindow">The duration of the resolution window.</param>
    public ContentionRules(decimal rejectionThreshold, TimeSpan resolutionWindow)
    {
        RejectionThreshold = rejectionThreshold;
        ResolutionWindow = resolutionWindow;
    }

    /// <summary>
    /// Gets the threshold for rejecting a membership request.
    /// </summary>
    public decimal RejectionThreshold { get; }

    /// <summary>
    /// Gets the duration of the resolution window for membership approval decisions.
    /// </summary>
    public TimeSpan ResolutionWindow { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RejectionThreshold;
        yield return ResolutionWindow;
    }
}
