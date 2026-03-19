using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SignalAwareness.Contracts;
using BindingChaos.SignalAwareness.Domain.Signals.Events;

namespace BindingChaos.SignalAwareness.Infrastructure.IntegrationEventMappers;

/// <summary>
/// Maps SignalCaptured domain events to SignalCapturedIntegrationEvent integration events.
/// </summary>
public class SignalCapturedIntegrationEventMapper : IIntegrationEventMapper<SignalCaptured>
{
    /// <summary>
    /// Maps a SignalCaptured domain event to a SignalCapturedIntegrationEvent.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped integration event.</returns>
    public IEnumerable<IIntegrationEvent> Map(SignalCaptured domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        yield return new SignalCapturedIntegrationEvent(
            SignalId: domainEvent.AggregateId,
            OriginatorId: domainEvent.OriginatorId,
            Latitude: domainEvent.Latitude,
            Longitude: domainEvent.Longitude,
            Title: domainEvent.Title,
            Description: domainEvent.Description,
            Tags: domainEvent.Tags,
            CapturedAt: domainEvent.OccurredAt);
    }
}
