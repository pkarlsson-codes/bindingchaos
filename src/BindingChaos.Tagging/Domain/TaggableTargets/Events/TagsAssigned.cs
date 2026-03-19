using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Domain.TaggableTargets.Events;

/// <summary>
/// Represents a domain event that records the assignment of one or more tags to an aggregate entity.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate to which the tags are assigned.</param>
/// <param name="Version">The version of the aggregate at the time the tags were assigned.</param>
/// <param name="TagIds">An array containing the identifiers of the tags that have been assigned to the aggregate.</param>
/// <param name="AddedByParticipantId">The identifier of the participant who performed the tag assignment.</param>
public sealed record TagsAssigned(
    string AggregateId,
    long Version,
    string[] TagIds,
    string AddedByParticipantId
    ) : DomainEvent(AggregateId, Version);
