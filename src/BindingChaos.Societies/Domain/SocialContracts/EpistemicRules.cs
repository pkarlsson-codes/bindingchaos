using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Societies.Domain.SocialContracts;

/// <summary>
/// Value object representing the epistemic rules that govern signal verification within a social contract.
/// </summary>
public sealed class EpistemicRules : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpistemicRules"/> class.
    /// </summary>
    /// <param name="requiredVerificationWeight">The minimum verification weight required for signals.</param>
    public EpistemicRules(double requiredVerificationWeight)
    {
        RequiredVerificationWeight = requiredVerificationWeight;
    }

    /// <summary>
    /// Gets the minimum verification weight required for signals to be considered valid.
    /// </summary>
    public double RequiredVerificationWeight { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RequiredVerificationWeight;
    }
}
