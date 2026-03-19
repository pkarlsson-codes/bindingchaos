using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Entity representing a single participant's amplification of a signal.
/// </summary>
internal sealed class SignalAmplification : Entity<SignalAmplificationId>
{
    private SignalAmplification(SignalAmplificationId id, SignalId signalId, ParticipantId amplifierId, AmplificationReason reason, string? commentary, DateTimeOffset amplifiedAt)
    {
        Id = id;
        SignalId = signalId;
        AmplifierId = amplifierId;
        Reason = reason;
        Commentary = commentary;
        AmplifiedAt = amplifiedAt;
        IsWithdrawn = false;
        WithdrawnAt = null;
    }

    /// <summary>
    /// Gets the associated signal identifier.
    /// </summary>
    public SignalId SignalId { get; private set; } = null!;

    /// <summary>
    /// Gets the participant who amplified the signal.
    /// </summary>
    public ParticipantId AmplifierId { get; private set; } = null!;

    /// <summary>
    /// Gets the reason selected for amplification.
    /// </summary>
    public AmplificationReason Reason { get; private set; }

    /// <summary>
    /// Gets optional commentary provided by the participant.
    /// </summary>
    public string? Commentary { get; private set; }

    /// <summary>
    /// Gets the timestamp when amplification occurred.
    /// </summary>
    public DateTimeOffset AmplifiedAt { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the amplification has been withdrawn.
    /// </summary>
    public bool IsWithdrawn { get; private set; }

    /// <summary>
    /// Gets the timestamp when the amplification was withdrawn, if withdrawn.
    /// </summary>
    public DateTimeOffset? WithdrawnAt { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this amplification is active (not withdrawn).
    /// </summary>
    public bool IsActive => !IsWithdrawn;

    /// <summary>
    /// Creates a new instance of the <see cref="SignalAmplification"/> class with the specified parameters.
    /// </summary>
    /// <param name="signalAmplificationId">The unique identifier for the signal amplification.</param>
    /// <param name="signalId">The identifier of the signal being amplified.</param>
    /// <param name="amplifierId">The identifier of the participant performing the amplification.</param>
    /// <param name="reason">The reason for the amplification.</param>
    /// <param name="commentary">Optional commentary or additional information about the amplification. Can be <see langword="null"/>.</param>
    /// <param name="amplifiedAt">The date and time when the amplification occurred.</param>
    /// <returns>A new <see cref="SignalAmplification"/> instance initialized with the provided parameters.</returns>
    public static SignalAmplification Create(
        SignalAmplificationId signalAmplificationId,
        SignalId signalId, ParticipantId amplifierId,
        AmplificationReason reason, string? commentary, DateTimeOffset amplifiedAt)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        ArgumentNullException.ThrowIfNull(amplifierId);
        ArgumentNullException.ThrowIfNull(reason);

        return new SignalAmplification(signalAmplificationId, signalId, amplifierId, reason, commentary, amplifiedAt);
    }

    /// <summary>
    /// Marks this amplification as withdrawn.
    /// </summary>
    /// <param name="attenuatedAt">The timestamp when withdrawal occurred.</param>
    public void Attenuate(DateTimeOffset attenuatedAt)
    {
        if (IsWithdrawn)
        {
            throw new BusinessRuleViolationException("Amplification has already been attenuated");
        }

        IsWithdrawn = true;
        WithdrawnAt = attenuatedAt;
    }
}