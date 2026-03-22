using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Domain.TaggableTargets.Events;

/// <summary>
/// Represents an event that records the removal of one or more tags from a specific aggregate.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate from which the tags were removed.</param>
/// <param name="TagIds">An array containing the identifiers of the tags that have been removed.</param>
/// <param name="AddedByParticipantId">The identifier of the participant who performed the tag removal.</param>
public sealed record TagsRemoved(
    string AggregateId,
    string[] TagIds,
    string AddedByParticipantId
    ) : DomainEvent(AggregateId);
