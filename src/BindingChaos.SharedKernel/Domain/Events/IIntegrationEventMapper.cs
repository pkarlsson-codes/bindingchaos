namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Interface for mapping domain events to integration events.
/// </summary>
/// <typeparam name="TDomainEvent">The type of the domain event to map.</typeparam>
public interface IIntegrationEventMapper<in TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Maps a domain event to one or more integration events.
    /// </summary>
    /// <param name="e">The domain event to map. This should be a specific domain event type that implements IDomainEvent.</param>
    /// <returns>An enumerable collection of integration events that represent the domain event.</returns>
    IEnumerable<IIntegrationEvent> Map(TDomainEvent e);
}

/// <summary>
/// Adapter interface for mapping domain events to integration events.
/// </summary>
public interface IIntegrationEventMapperAdapter
{
    /// <summary>
    /// Checks if the mapper can handle the given domain event.
    /// </summary>
    /// <param name="e">The domain event to check.</param>
    /// <returns>True if the mapper can handle the event; otherwise, false.</returns>
    bool CanHandle(IDomainEvent e);

    /// <summary>
    /// Maps a domain event to one or more integration events.
    /// </summary>
    /// <param name="e">The domain event to map.</param>
    /// <returns>An enumerable collection of integration events that represent the domain event.</returns>
    IEnumerable<IIntegrationEvent> Map(IDomainEvent e);
}

/// <summary>
/// Adapter for mapping a specific type of domain event to integration events.
/// </summary>
/// <typeparam name="TDomainEvent">The type of the domain event to map.</typeparam>
public sealed class IntegrationEventMapperAdapter<TDomainEvent>(IIntegrationEventMapper<TDomainEvent> inner) : IIntegrationEventMapperAdapter
    where TDomainEvent : IDomainEvent
{
    /// <inheritdoc/>
    public bool CanHandle(IDomainEvent e) => e is TDomainEvent;

    /// <inheritdoc/>
    public IEnumerable<IIntegrationEvent> Map(IDomainEvent e)
        => inner.Map((TDomainEvent)e);
}
