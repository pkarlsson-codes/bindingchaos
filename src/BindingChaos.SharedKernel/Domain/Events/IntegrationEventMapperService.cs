namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Registry for mapping domain events to integration events.
/// </summary>
/// <param name="adapters">The collection of integration event mapper adapters.</param>
public class IntegrationEventMapperService(IEnumerable<IIntegrationEventMapperAdapter> adapters) : IIntegrationEventMapperService
{
    private readonly IEnumerable<IIntegrationEventMapperAdapter> _adapters = adapters ?? throw new ArgumentNullException(nameof(adapters));

    /// <inheritdoc />
    public IEnumerable<IIntegrationEvent> Map(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        var matchingAdapters = _adapters.Where(a => a.CanHandle(domainEvent)).ToList();
        var integrationEvents = matchingAdapters.SelectMany(a => a.Map(domainEvent)).ToList();
        return integrationEvents;
    }
}
