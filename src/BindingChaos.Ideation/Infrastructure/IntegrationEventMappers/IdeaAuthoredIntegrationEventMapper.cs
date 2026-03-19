using BindingChaos.Ideation.Contracts;
using BindingChaos.Ideation.Domain.Ideas.Events;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Infrastructure.IntegrationEventMappers;

/// <summary>
/// Maps IdeaAuthored domain events to IdeaAuthoredIntegrationEvent integration events.
/// </summary>
public class IdeaAuthoredIntegrationEventMapper : IIntegrationEventMapper<IdeaAuthored>
{
    /// <summary>
    /// Maps an IdeaAuthored domain event to an IdeaAuthoredIntegrationEvent.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped integration event.</returns>
    public IEnumerable<IIntegrationEvent> Map(IdeaAuthored domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        yield return new IdeaAuthoredIntegrationEvent(
            IdeaId: domainEvent.AggregateId,
            AuthorId: domainEvent.AuthorId,
            SocietyContext: domainEvent.SocietyContext,
            Title: domainEvent.Title,
            Body: domainEvent.Body,
            SignalReferences: domainEvent.SignalReferences,
            Tags: domainEvent.Tags,
            AuthoredAt: domainEvent.OccurredAt,
            ParentIdeaId: domainEvent.ParentIdeaId);
    }
}