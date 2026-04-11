using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Publishes <see cref="ProjectInquiryRaisedIntegrationEvent"/> when a <see cref="ProjectInquiryRaised"/> domain event is processed.
/// </summary>
public static class ProjectInquiryRaisedHandler
{
    /// <summary>
    /// Handles a <see cref="ProjectInquiryRaised"/> domain event by publishing the corresponding integration event.
    /// </summary>
    /// <param name="e">The domain event.</param>
    /// <param name="messageBus">The message bus for publishing integration events.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ProjectInquiryRaised e,
        IMessageBus messageBus)
    {
        await messageBus.PublishAsync(
                new ProjectInquiryRaisedIntegrationEvent(e.AggregateId, e.ProjectId))
            .ConfigureAwait(false);
    }
}
