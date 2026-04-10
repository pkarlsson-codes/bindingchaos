using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.Ideas.Events;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// A <see cref="IdeaDrafted"/> event handler.
/// </summary>
public static class IdeaDraftedHandler
{
    /// <summary>
    /// Handles a <see cref="IdeaDrafted"/> event.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="messageBus">A message bus.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        IdeaDrafted e,
        IMessageBus messageBus)
    {
        await messageBus.PublishAsync(
                new IdeaDraftedIntegrationEvent(
                    e.AggregateId,
                    e.AuthorId,
                    e.Title,
                    e.Description))
            .ConfigureAwait(false);
    }
}
