using BindingChaos.Ideation.Contracts;
using BindingChaos.Ideation.Domain.Amendments.Events;
using BindingChaos.SharedKernel.Domain.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Ideation.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Message handler for AmendmentProposed domain events that publishes the corresponding integration event.
/// This handler receives domain events from Marten's async daemon and publishes them via Wolverine's message bus.
/// </summary>
public class AmendmentProposedHandler
{
    private readonly IIntegrationEventMapper<AmendmentProposed> _mapper;
    private readonly ILogger<AmendmentProposedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the AmendmentProposedHandler.
    /// </summary>
    /// <param name="mapper">The mapper for converting domain events to integration events.</param>
    /// <param name="logger">The logger instance.</param>
    public AmendmentProposedHandler(
        IIntegrationEventMapper<AmendmentProposed> mapper,
        ILogger<AmendmentProposedHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the AmendmentProposed domain event by mapping and publishing the integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="messageBus">The message bus for publishing the integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(AmendmentProposed domainEvent, IMessageBus messageBus)
    {
        try
        {
            var integrationEvent = _mapper.Map(domainEvent).FirstOrDefault();
            if (integrationEvent is AmendmentProposedIntegrationEvent)
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
                "Published AmendmentProposedIntegrationEvent for amendment {AmendmentId}");

        private static readonly Action<ILogger, string, Exception?> Error =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(2, nameof(Error)),
                "Error publishing integration event for AmendmentProposed domain event {AmendmentId}");

        internal static void PublishedEvent(ILogger logger, string amendmentId) =>
            Published(logger, amendmentId, null);

        internal static void ErrorEvent(ILogger logger, string amendmentId, Exception? ex) =>
            Error(logger, amendmentId, ex);
    }
}
