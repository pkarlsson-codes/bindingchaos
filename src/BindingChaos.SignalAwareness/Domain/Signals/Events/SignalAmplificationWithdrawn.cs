using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Domain event raised when a signal amplification is attenuated by a participant.
/// </summary>
/// <param name="AggregateId">The signal identifier.</param>
/// <param name="AmplificationId">The amplification identifier being attenuated.</param>
/// <param name="AmplifierId">The participant who attenuated their amplification.</param>
internal sealed record SignalAmplificationAttenuated(
    string AggregateId,
    string AmplificationId,
    string AmplifierId
) : DomainEvent(AggregateId);
