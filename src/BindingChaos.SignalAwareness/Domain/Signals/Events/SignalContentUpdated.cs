using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Event raised when a signal's content is updated.
/// </summary>
/// <param name="AggregateId">The signal identifier.</param>
/// <param name="Version">Aggregate version at the time of the event.</param>
/// <param name="Title">The new title.</param>
/// <param name="Description">The new description.</param>
internal sealed record SignalContentUpdated(
    string AggregateId,
    long Version,
    string Title,
    string Description
) : DomainEvent(AggregateId, Version);
