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
