using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Represents a domain event that occurs when tags are added to a signal.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate to which the tags were added.</param>
/// <param name="Tags">An array of tags that were added to the signal. This array cannot be null or empty.</param>
internal sealed record TagsAddedToSignal(
    string AggregateId,
    string[] Tags
) : DomainEvent(AggregateId);
