using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for <see cref="CommonsCreated"/> domain events that publishes the corresponding integration event.
/// </summary>
public sealed class CommonsCreatedHandler
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

    private static class Logs
    {
        private static readonly Action<ILogger, string, string, Exception?> Handling =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(1, nameof(Handling)),
                "Handling CommonsCreated domain event for commons {CommonsId}, founder {FounderId}");

        private static readonly Action<ILogger, string, Exception?> Publishing =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(2, nameof(Publishing)),
                "Publishing CommonsCreatedIntegrationEvent for commons {CommonsId}");

        private static readonly Action<ILogger, string, Exception?> Published =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(3, nameof(Published)),
                "Published CommonsCreatedIntegrationEvent for commons {CommonsId}");

        private static readonly Action<ILogger, string, Exception?> NoMappedEvent =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(4, nameof(NoMappedEvent)),
                "No CommonsCreatedIntegrationEvent was mapped for commons {CommonsId}");

        private static readonly Action<ILogger, string, Exception?> Error =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(5, nameof(Error)),
                "Error publishing integration event for CommonsCreated domain event {CommonsId}");

        internal static void HandlingDomainEvent(ILogger logger, string commonsId, string founderId) =>
            Handling(logger, commonsId, founderId, null);

        internal static void PublishingIntegrationEvent(ILogger logger, string commonsId) =>
            Publishing(logger, commonsId, null);

        internal static void PublishedEvent(ILogger logger, string commonsId) =>
            Published(logger, commonsId, null);

        internal static void NoIntegrationEventMapped(ILogger logger, string commonsId) =>
            NoMappedEvent(logger, commonsId, null);

        internal static void ErrorEvent(ILogger logger, string commonsId, Exception? ex) =>
            Error(logger, commonsId, ex);
    }
}
