using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Base record for integration events that cross bounded context boundaries.
/// Integration events represent something that happened in one bounded context
/// that other bounded contexts might be interested in, without creating direct dependencies.
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier for this integration event.
    /// </summary>
    public string EventId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets the timestamp when this integration event occurred.
    /// </summary>
    public DateTimeOffset OccurredAt { get; init; } = TimeProviderContext.Current.UtcNow;

    /// <summary>
    /// Gets the name of the bounded context that published this event.
    /// </summary>
    public virtual string SourceBoundedContext => GetType().Namespace?.Split('.')[1] ?? "Unknown"; // TODO: Yuck, this seems very fragile and undocumented.

    /// <inheritdoc />
    public override string ToString() => $"{GetType().Name}[{EventId}]";
}