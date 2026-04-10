using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Societies.Domain.SocialContracts;

/// <summary>
/// Value object representing the decision-making protocol for a social contract.
/// </summary>
public sealed class DecisionProtocol : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecisionProtocol"/> class.
    /// </summary>
    /// <param name="ratificationThreshold">The percentage of total weight required to ratify a decision (e.g., 0.5 for 50%).</param>
    /// <param name="reviewWindow">The duration an amendment must remain open for feedback or objection.</param>
    /// <param name="allowVeto">If true, a single principled objection can block a decision regardless of weight.</param>
    /// <param name="inquiryLapseWindow">How long an unanswered or unaccepted inquiry remains open before auto-lapsing.</param>
    public DecisionProtocol(double ratificationThreshold, TimeSpan reviewWindow, bool allowVeto, TimeSpan inquiryLapseWindow)
    {
        RatificationThreshold = ratificationThreshold;
        ReviewWindow = reviewWindow;
        AllowVeto = allowVeto;
        InquiryLapseWindow = inquiryLapseWindow;
    }

    /// <summary>
    /// Gets the percentage of total participant weight required to ratify an amendment.
    /// </summary>
    public double RatificationThreshold { get; }

    /// <summary>
    /// Gets the duration an amendment must remain open for feedback or objection.
    /// </summary>
    public TimeSpan ReviewWindow { get; }

    /// <summary>
    /// Gets a value indicating whether a single principled objection can block a decision.
    /// </summary>
    public bool AllowVeto { get; }

    /// <summary>Gets how long an unanswered or unaccepted inquiry remains open before auto-lapsing.</summary>
    public TimeSpan InquiryLapseWindow { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RatificationThreshold;
        yield return ReviewWindow;
        yield return AllowVeto;
        yield return InquiryLapseWindow;
    }
}
