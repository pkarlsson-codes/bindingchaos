using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Entity representing a single participant's amplification of a signal.
/// </summary>
internal sealed class SignalAmplification : Entity<SignalAmplificationId>
{
    private SignalAmplification(SignalAmplificationId id, ParticipantId amplifierId)
    {
        Id = id;
        AmplifierId = amplifierId;
        IsWithdrawn = false;
    }

    /// <summary>
    /// Gets the participant who amplified the signal.
    /// </summary>
    public ParticipantId AmplifierId { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the amplification has been withdrawn.
    /// </summary>
    public bool IsWithdrawn { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this amplification is active (not withdrawn).
    /// </summary>
    public bool IsActive => !IsWithdrawn;

    /// <summary>
    /// Creates a new instance of the <see cref="SignalAmplification"/> class.
    /// </summary>
    /// <param name="signalAmplificationId">The unique identifier for the signal amplification.</param>
    /// <param name="amplifierId">The identifier of the participant performing the amplification.</param>
    /// <returns>A new <see cref="SignalAmplification"/> instance.</returns>
    public static SignalAmplification Create(SignalAmplificationId signalAmplificationId, ParticipantId amplifierId)
    {
        ArgumentNullException.ThrowIfNull(amplifierId);

        return new SignalAmplification(signalAmplificationId, amplifierId);
    }

    /// <summary>
    /// Marks this amplification as withdrawn.
    /// </summary>
    public void Attenuate()
    {
        if (IsWithdrawn)
        {
            throw new BusinessRuleViolationException("Amplification has already been attenuated");
        }

        IsWithdrawn = true;
    }
}
