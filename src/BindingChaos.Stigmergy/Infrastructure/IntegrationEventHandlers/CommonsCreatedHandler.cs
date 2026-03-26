using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for <see cref="CommonsCreated"/> domain events that publishes the corresponding integration event.
/// </summary>
public sealed partial class CommonsCreatedHandler
{
    private readonly IIntegrationEventMapper<CommonsCreated> _mapper;
    private readonly ILogger<CommonsCreatedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CommonsCreatedHandler"/>.
    /// </summary>
    /// <param name="mapper">The mapper for converting domain events to integration events.</param>
    /// <param name="logger">The logger instance.</param>
    public CommonsCreatedHandler(
        IIntegrationEventMapper<CommonsCreated> mapper,
        ILogger<CommonsCreatedHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the <see cref="CommonsCreated"/> domain event by mapping and publishing the integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="messageBus">The message bus for publishing the integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(CommonsCreated domainEvent, IMessageBus messageBus)
    {
        Logs.HandlingDomainEvent(_logger, domainEvent.AggregateId, domainEvent.FounderId);

        try
        {
            var integrationEvent = _mapper.Map(domainEvent).FirstOrDefault();
            if (integrationEvent is CommonsCreatedIntegrationEvent)
            {
                Logs.PublishingIntegrationEvent(_logger, domainEvent.AggregateId);
                await messageBus.PublishAsync(integrationEvent).ConfigureAwait(false);
                Logs.PublishedEvent(_logger, domainEvent.AggregateId);
                return;
            }

            Logs.NoIntegrationEventMapped(_logger, domainEvent.AggregateId);
        }
        catch (Exception ex)
        {
            Logs.ErrorEvent(_logger, domainEvent.AggregateId, ex);
            throw;
        }
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Handling CommonsCreated domain event for commons {CommonsId}, founder {FounderId}")]
        internal static partial void HandlingDomainEvent(ILogger logger, string commonsId, string founderId);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Publishing CommonsCreatedIntegrationEvent for commons {CommonsId}")]
        internal static partial void PublishingIntegrationEvent(ILogger logger, string commonsId);

        [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Published CommonsCreatedIntegrationEvent for commons {CommonsId}")]
        internal static partial void PublishedEvent(ILogger logger, string commonsId);

        [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "No CommonsCreatedIntegrationEvent was mapped for commons {CommonsId}")]
        internal static partial void NoIntegrationEventMapped(ILogger logger, string commonsId);

        [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Error publishing integration event for CommonsCreated domain event {CommonsId}")]
        internal static partial void ErrorEvent(ILogger logger, string commonsId, Exception? exception);
    }
}
