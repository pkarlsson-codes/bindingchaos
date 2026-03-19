using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Event raised when a signal's status is changed.
/// </summary>
/// <param name="AggregateId">The signal identifier.</param>
/// <param name="Version">Aggregate version at the time of the event.</param>
/// <param name="NewStatus">The new status value.</param>
internal sealed record SignalStatusChanged(
    string AggregateId,
    long Version,
    int NewStatus
) : DomainEvent(AggregateId, Version);