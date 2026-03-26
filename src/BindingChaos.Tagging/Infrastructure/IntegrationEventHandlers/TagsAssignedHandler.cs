using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Tagging.Contracts;
using BindingChaos.Tagging.Domain.TaggableTargets.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Tagging.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for TagsAssigned domain events that publishes the corresponding integration event.
/// This handler receives domain events from Marten's async daemon and publishes them via Wolverine's message bus.
/// </summary>
public sealed partial class TagsAssignedHandler
{
    private readonly IIntegrationEventMapper<TagsAssigned> _mapper;
    private readonly ILogger<TagsAssignedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the TagsAssignedHandler.
    /// </summary>
    /// <param name="mapper">The mapper for converting domain events to integration events.</param>
    /// <param name="logger">The logger instance.</param>
    public TagsAssignedHandler(
        IIntegrationEventMapper<TagsAssigned> mapper,
        ILogger<TagsAssignedHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the TagsAssigned domain event by mapping and publishing the integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="messageBus">The message bus for publishing the integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(TagsAssigned domainEvent, IMessageBus messageBus)
    {
        try
        {
            var integrationEvent = _mapper.Map(domainEvent).FirstOrDefault();
            if (integrationEvent is TagsAssignedIntegrationEvent)
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

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Published TagsAssignedIntegrationEvent for target {TargetId}")]
        internal static partial void PublishedEvent(ILogger logger, string targetId);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error publishing integration event for TagsAssigned domain event {TargetId}")]
        internal static partial void ErrorEvent(ILogger logger, string targetId, Exception? exception);
    }
}
