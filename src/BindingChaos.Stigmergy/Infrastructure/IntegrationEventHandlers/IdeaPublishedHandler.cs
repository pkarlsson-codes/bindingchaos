using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.Ideas.Events;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// A <see cref="IdeaPublished"/> event handler.
/// </summary>
public static class IdeaPublishedHandler
{
    /// <summary>
    /// Handles a <see cref="IdeaPublished"/> event.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="messageBus">A message bus.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        IdeaPublished e,
        IMessageBus messageBus)
    {
        await messageBus.PublishAsync(
            new IdeaPublishedIntegrationEvent(
                e.AggregateId,
                e.PublishedById))
            .ConfigureAwait(false);
    }
}