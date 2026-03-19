using BindingChaos.Ideation.Contracts;
using BindingChaos.Ideation.Domain.Amendments.Events;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Infrastructure.IntegrationEventMappers;

/// <summary>
/// Maps AmendmentProposed domain events to AmendmentProposedIntegrationEvent integration events.
/// </summary>
public class AmendmentProposedIntegrationEventMapper : IIntegrationEventMapper<AmendmentProposed>
{
    /// <summary>
    /// Maps an AmendmentProposed domain event to an AmendmentProposedIntegrationEvent.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped integration event.</returns>
    public IEnumerable<IIntegrationEvent> Map(AmendmentProposed domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        yield return new AmendmentProposedIntegrationEvent(
            AmendmentId: domainEvent.AggregateId,
            TargetIdeaId: domainEvent.TargetIdeaId,
            CreatorId: domainEvent.CreatorId,
            ProposedTitle: domainEvent.ProposedTitle,
            ProposedBody: domainEvent.ProposedBody,
            AmendmentTitle: domainEvent.AmendmentTitle,
            AmendmentDescription: domainEvent.AmendmentDescription,
            ProposedAt: domainEvent.OccurredAt);
    }
}
