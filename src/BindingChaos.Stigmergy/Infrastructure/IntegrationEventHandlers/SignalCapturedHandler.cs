using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.Signals.Events;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// A <see cref="SignalCaptured"/> event handler.
/// </summary>
public static class SignalCapturedHandler
{
    /// <summary>
    /// Handles a <see cref="SignalCaptured"/> event.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="messageBus">A message bus.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        SignalCaptured e,
        IMessageBus messageBus)
    {
        await messageBus.PublishAsync(
                new SignalCapturedIntegrationEvent(
                    e.AggregateId,
                    e.CapturedById,
                    e.Description,
                    e.Tags))
            .ConfigureAwait(false);
    }
}