namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Registry for mapping domain events to integration events.
/// </summary>
public interface IIntegrationEventMapperService
{
    /// <summary>
    /// Maps a domain event to one or more integration events.
    /// </summary>
    /// <param name="domainEvent">The domain event instance to map.</param>
    /// <returns>A collection of integration events mapped from the domain event.</returns>
    IEnumerable<IIntegrationEvent> Map(IDomainEvent domainEvent);
}
