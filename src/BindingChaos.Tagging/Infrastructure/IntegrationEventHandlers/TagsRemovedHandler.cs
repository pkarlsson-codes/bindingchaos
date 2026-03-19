using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Tagging.Contracts;
using BindingChaos.Tagging.Domain.TaggableTargets.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Tagging.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for TagsRemoved domain events that publishes the corresponding integration event.
/// This handler receives domain events from Marten's async daemon and publishes them via Wolverine's message bus.
/// </summary>
public class TagsRemovedHandler
{
    private readonly IIntegrationEventMapper<TagsRemoved> _mapper;
    private readonly ILogger<TagsRemovedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the TagsRemovedHandler.
    /// </summary>
    /// <param name="mapper">The mapper for converting domain events to integration events.</param>
    /// <param name="logger">The logger instance.</param>
    public TagsRemovedHandler(
        IIntegrationEventMapper<TagsRemoved> mapper,
        ILogger<TagsRemovedHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the TagsRemoved domain event by mapping and publishing the integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="messageBus">The message bus for publishing the integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(TagsRemoved domainEvent, IMessageBus messageBus)
    {
        try
        {
            var integrationEvent = _mapper.Map(domainEvent).FirstOrDefault();
            if (integrationEvent is TagsRemovedIntegrationEvent)
            {
                await messageBus.PublishAsync(integrationEvent).ConfigureAwait(false);
                Logs.PublishedEvent(_logger, domainEvent.AggregateId);
            }
        }
        catch (Exception ex)
        {
            Logs.ErrorEvent(_logger, domainEvent.AggregateId, ex);
            throw;
        }
    }

    private static class Logs
    {
        private static readonly Action<ILogger, string, Exception?> Published =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1, nameof(Published)),
                "Published TagsRemovedIntegrationEvent for target {TargetId}");

        private static readonly Action<ILogger, string, Exception?> Error =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(2, nameof(Error)),
                "Error publishing integration event for TagsRemoved domain event {TargetId}");

        internal static void PublishedEvent(ILogger logger, string targetId) =>
            Published(logger, targetId, null);

        internal static void ErrorEvent(ILogger logger, string targetId, Exception? ex) =>
            Error(logger, targetId, ex);
    }
}
