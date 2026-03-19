using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Domain event raised when a signal amplification is attenuated by a participant.
/// </summary>
/// <param name="AggregateId">The signal identifier.</param>
/// <param name="Version">Aggregate version at the time of the event.</param>
/// <param name="AmplificationId">The amplification identifier being attenuated.</param>
/// <param name="AmplifierId">The participant who attenuated their amplification.</param>
internal sealed record SignalAmplificationAttenuated(
    string AggregateId,
    long Version,
    string AmplificationId,
    string AmplifierId
) : DomainEvent(AggregateId, Version);
