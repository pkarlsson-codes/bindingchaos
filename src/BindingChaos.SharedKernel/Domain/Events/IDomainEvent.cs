namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the timestamp when this domain event occurred.
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Gets the ID of the aggregate that raised this event.
    /// </summary>
    string AggregateId { get; }

    /// <summary>
    /// Gets the version of the aggregate when this event was raised.
    /// </summary>
    long Version { get; }
}