using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Signals.Events;

/// <summary>
/// Signal amplification withdrawn event.
/// </summary>
/// <param name="AggregateId">Id of the signal that amplification was withdrawn from.</param>
/// <param name="AmplifierId">Id of the actor withdrawing amplification.</param>
public record SignalAmplificationWithdrawn(
    string AggregateId,
    string AmplifierId
) : DomainEvent(AggregateId);