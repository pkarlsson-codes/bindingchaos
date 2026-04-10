using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for <see cref="CommonsCreated"/> domain events that publishes the corresponding integration event.
/// </summary>
public static class CommonsCreatedHandler
{
    /// <summary>
    /// Handles the <see cref="CommonsCreated"/> domain event by mapping and publishing the integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="messageBus">The message bus for publishing the integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(CommonsCreated domainEvent, IMessageBus messageBus)
    {
        await messageBus.PublishAsync(
            new CommonsCreatedIntegrationEvent(
                CommonsId: domainEvent.AggregateId,
                Name: domainEvent.Name,
                Description: domainEvent.Description,
                FounderId: domainEvent.FounderId,
                ProposedAt: domainEvent.OccurredAt))
            .ConfigureAwait(false);
    }
}
