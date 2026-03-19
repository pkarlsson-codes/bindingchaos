using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Tagging.Contracts;
using BindingChaos.Tagging.Domain.TaggableTargets;
using BindingChaos.Tagging.Domain.TaggableTargets.Events;

namespace BindingChaos.Tagging.Infrastructure.IntegrationEventMappers;

/// <summary>
/// Provides functionality to map a TagsAssigned domain event to one or more integration events for external systems.
/// </summary>
public class TagsAssignedIntegrationEventMapper : IIntegrationEventMapper<TagsAssigned>
{
    /// <summary>
    /// Maps an AmendmentProposed domain event to an AmendmentProposedIntegrationEvent.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped integration event.</returns>
    public IEnumerable<IIntegrationEvent> Map(TagsAssigned domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        yield return new TagsAssignedIntegrationEvent(
            TaggableTargetId.Create(domainEvent.AggregateId).EntityId,
            domainEvent.TagIds);
    }
}
