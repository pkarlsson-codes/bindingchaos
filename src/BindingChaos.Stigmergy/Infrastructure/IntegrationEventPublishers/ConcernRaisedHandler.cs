using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.Concerns.Events;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// A <see cref="ConcernRaised"/> event handler.
/// </summary>
public static class ConcernRaisedHandler
{
    /// <summary>
    /// Handles a <see cref="ConcernRaised"/> event.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="messageBus">A message bus.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        ConcernRaised e,
        IMessageBus messageBus)
    {
        await messageBus.PublishAsync(
            new ConcernRaisedIntegrationEvent(
                e.AggregateId,
                e.ActorId,
                e.Name))
            .ConfigureAwait(false);
    }
}