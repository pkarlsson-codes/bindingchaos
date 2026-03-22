using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Event raised when a signal's content is updated.
/// </summary>
/// <param name="AggregateId">The signal identifier.</param>
/// <param name="Title">The new title.</param>
/// <param name="Description">The new description.</param>
internal sealed record SignalContentUpdated(
    string AggregateId,
    string Title,
    string Description
) : DomainEvent(AggregateId);
