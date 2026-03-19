using BindingChaos.Ideation.Contracts;
using BindingChaos.Ideation.Domain.Ideas.Events;
using BindingChaos.SharedKernel.Domain.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Ideation.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for IdeaAuthored domain events that publishes the corresponding integration event.
/// This handler receives domain events from Marten's async daemon and publishes them via Wolverine's message bus.
/// </summary>
public class IdeaAuthoredHandler
{
    private readonly IIntegrationEventMapper<IdeaAuthored> _mapper;
    private readonly ILogger<IdeaAuthoredHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the IdeaAuthoredHandler.
    /// </summary>
    /// <param name="mapper">The mapper for converting domain events to integration events.</param>
    /// <param name="logger">The logger instance.</param>
    public IdeaAuthoredHandler(
        IIntegrationEventMapper<IdeaAuthored> mapper,
        ILogger<IdeaAuthoredHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the IdeaAuthored domain event by mapping and publishing the integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="messageBus">The message bus for publishing the integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(IdeaAuthored domainEvent, IMessageBus messageBus)
    {
        try
        {
            var integrationEvent = _mapper.Map(domainEvent).FirstOrDefault();
            if (integrationEvent is IdeaAuthoredIntegrationEvent)
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
                "Published IdeaAuthoredIntegrationEvent for idea {IdeaId}");

        private static readonly Action<ILogger, string, Exception?> Error =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(2, nameof(Error)),
                "Error publishing integration event for IdeaAuthored domain event {IdeaId}");

        internal static void PublishedEvent(ILogger logger, string ideaId) =>
            Published(logger, ideaId, null);

        internal static void ErrorEvent(ILogger logger, string ideaId, Exception? ex) =>
            Error(logger, ideaId, ex);
    }
}
