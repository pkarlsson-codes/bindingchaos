namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Marker interface for integration events that cross bounded context boundaries.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier for this integration event.
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// Gets the timestamp when this integration event occurred.
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Gets the name of the bounded context that published this event.
    /// </summary>
    string SourceBoundedContext { get; }
}
