using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Signals;

/// <summary>
/// A signal amplification.
/// </summary>
public class Amplification : ValueObject
{
    /// <summary>
    /// Creates an instance of <see cref="Amplification"/>.
    /// </summary>
    /// <param name="amplifiedById">Id of the actor that amplified the signal.</param>
    internal Amplification(ParticipantId amplifiedById)
    {
        AmplifiedById = amplifiedById;
    }

    /// <summary>
    /// Id of actor that amplified the signal.
    /// </summary>
    public ParticipantId AmplifiedById { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AmplifiedById;
    }
}