using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SignalAwareness.Contracts;
using BindingChaos.SignalAwareness.Domain.Signals.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.SignalAwareness.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for SignalCaptured domain events that publishes the corresponding integration event.
/// This handler receives domain events from Marten's async daemon and publishes them via Wolverine's message bus.
/// </summary>
public class SignalCapturedHandler
{
    private readonly IIntegrationEventMapper<SignalCaptured> _mapper;
    private readonly ILogger<SignalCapturedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the SignalCapturedHandler.
    /// </summary>
    /// <param name="mapper">The mapper for converting domain events to integration events.</param>
    /// <param name="logger">The logger instance.</param>
    public SignalCapturedHandler(
        IIntegrationEventMapper<SignalCaptured> mapper,
        ILogger<SignalCapturedHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the SignalCaptured domain event by mapping and publishing the integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="messageBus">The message bus for publishing the integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(SignalCaptured domainEvent, IMessageBus messageBus)
    {
        Logs.HandlingDomainEvent(_logger, domainEvent.AggregateId, domainEvent.OriginatorId, domainEvent.Tags?.Length ?? 0);

        try
        {
            var integrationEvent = _mapper.Map(domainEvent).FirstOrDefault();
            if (integrationEvent is SignalCapturedIntegrationEvent)
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
        private static readonly Action<ILogger, string, string, int, Exception?> Handling =
            LoggerMessage.Define<string, string, int>(
                LogLevel.Information,
                new EventId(1, nameof(Handling)),
                "Handling SignalCaptured domain event for signal {SignalId}, originator {OriginatorId}, tags {TagsCount}");

        private static readonly Action<ILogger, string, Exception?> Publishing =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(2, nameof(Publishing)),
                "Publishing SignalCapturedIntegrationEvent for signal {SignalId}");

        private static readonly Action<ILogger, string, Exception?> Published =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(3, nameof(Published)),
                "Published SignalCapturedIntegrationEvent for signal {SignalId}");

        private static readonly Action<ILogger, string, Exception?> NoMappedEvent =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(4, nameof(NoMappedEvent)),
                "No SignalCapturedIntegrationEvent was mapped for signal {SignalId}");

        private static readonly Action<ILogger, string, Exception?> Error =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(5, nameof(Error)),
                "Error publishing integration event for SignalCaptured domain event {SignalId}");

        internal static void HandlingDomainEvent(ILogger logger, string signalId, string originatorId, int tagsCount) =>
            Handling(logger, signalId, originatorId, tagsCount, null);

        internal static void PublishingIntegrationEvent(ILogger logger, string signalId) =>
            Publishing(logger, signalId, null);

        internal static void PublishedEvent(ILogger logger, string signalId) =>
            Published(logger, signalId, null);

        internal static void NoIntegrationEventMapped(ILogger logger, string signalId) =>
            NoMappedEvent(logger, signalId, null);

        internal static void ErrorEvent(ILogger logger, string signalId, Exception? ex) =>
            Error(logger, signalId, ex);
    }
}
