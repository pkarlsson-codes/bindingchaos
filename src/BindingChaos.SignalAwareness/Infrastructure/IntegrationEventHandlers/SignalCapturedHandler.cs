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
public sealed partial class SignalCapturedHandler
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

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Handling SignalCaptured domain event for signal {SignalId}, originator {OriginatorId}, tags {TagsCount}")]
        internal static partial void HandlingDomainEvent(ILogger logger, string signalId, string originatorId, int tagsCount);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Publishing SignalCapturedIntegrationEvent for signal {SignalId}")]
        internal static partial void PublishingIntegrationEvent(ILogger logger, string signalId);

        [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Published SignalCapturedIntegrationEvent for signal {SignalId}")]
        internal static partial void PublishedEvent(ILogger logger, string signalId);

        [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "No SignalCapturedIntegrationEvent was mapped for signal {SignalId}")]
        internal static partial void NoIntegrationEventMapped(ILogger logger, string signalId);

        [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Error publishing integration event for SignalCaptured domain event {SignalId}")]
        internal static partial void ErrorEvent(ILogger logger, string signalId, Exception? exception);
    }
}
