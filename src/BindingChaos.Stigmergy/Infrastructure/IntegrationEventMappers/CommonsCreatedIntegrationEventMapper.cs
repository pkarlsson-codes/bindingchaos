using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventMappers;

/// <summary>
/// Maps <see cref="CommonsCreated"/> domain events to <see cref="CommonsCreatedIntegrationEvent"/> integration events.
/// </summary>
public sealed class CommonsCreatedIntegrationEventMapper : IIntegrationEventMapper<CommonsCreated>
{
    /// <inheritdoc/>
    public IEnumerable<IIntegrationEvent> Map(CommonsCreated domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        yield return new CommonsCreatedIntegrationEvent(
            CommonsId: domainEvent.AggregateId,
            Name: domainEvent.Name,
            Description: domainEvent.Description,
            FounderId: domainEvent.FounderId,
            ProposedAt: domainEvent.OccurredAt);
    }
}
