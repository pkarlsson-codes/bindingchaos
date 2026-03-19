using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Domain event raised when a signal is amplified by a participant.
/// </summary>
/// <param name="AggregateId">The ID of the signal being amplified.</param>
/// <param name="Version">The version of the signal when this event occurred.</param>
/// <param name="AmplificationId">The unique ID of the amplification action.</param>
/// <param name="AmplifierId">The ID of the participant who amplified the signal.</param>
/// <param name="Reason">The reason for amplification, represented as an integer code.</param>
/// <param name="Commentary">Optional commentary provided by the amplifier.</param>
internal sealed record SignalAmplified(
    string AggregateId,
    long Version,
    string AmplificationId,
    string AmplifierId,
    int Reason,
    string? Commentary
) : DomainEvent(AggregateId, Version);