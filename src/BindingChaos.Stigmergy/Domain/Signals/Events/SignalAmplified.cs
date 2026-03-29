using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Signals.Events;

/// <summary>
/// Signal amplified event.
/// </summary>
/// <param name="AggregateId">Id of the signal that was amplified.</param>
/// <param name="AmplifiedById">Id of the actor that amplified the signal.</param>
public record SignalAmplified(
    string AggregateId,
    string AmplifiedById
) : DomainEvent(AggregateId);