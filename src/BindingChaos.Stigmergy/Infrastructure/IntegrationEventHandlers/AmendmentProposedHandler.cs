using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.Projects.Events;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// An <see cref="AmendmentProposed"/> event handler.
/// </summary>
internal static class AmendmentProposedHandler
{
    /// <summary>
    /// Handles an <see cref="AmendmentProposed"/> event.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="messageBus">A message bus.</param>
    /// <returns>A task.</returns>
    internal static async Task Handle(
        AmendmentProposed e,
        IMessageBus messageBus)
    {
        await messageBus.PublishAsync(
                new AmendmentProposedIntegrationEvent(
                    e.AmendmentId,
                    e.ProjectId,
                    e.ProposedBy,
                    e.ProposedAt))
            .ConfigureAwait(false);
    }
}
