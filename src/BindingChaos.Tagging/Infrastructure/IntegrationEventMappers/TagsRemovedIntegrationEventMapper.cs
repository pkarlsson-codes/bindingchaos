using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Tagging.Contracts;
using BindingChaos.Tagging.Domain.TaggableTargets;
using BindingChaos.Tagging.Domain.TaggableTargets.Events;

namespace BindingChaos.Tagging.Infrastructure.IntegrationEventMappers;

/// <summary>
/// Provides functionality to map a TagsRemoved domain event to its corresponding integration event for use in
/// distributed systems or external communication.
/// </summary>
public class TagsRemovedIntegrationEventMapper : IIntegrationEventMapper<TagsRemoved>
{
    /// <summary>
    /// Maps an AmendmentProposed domain event to an AmendmentProposedIntegrationEvent.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped integration event.</returns>
    public IEnumerable<IIntegrationEvent> Map(TagsRemoved domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        yield return new TagsRemovedIntegrationEvent(
            TaggableTargetId.Create(domainEvent.AggregateId).EntityId,
            domainEvent.TagIds);
    }
}
