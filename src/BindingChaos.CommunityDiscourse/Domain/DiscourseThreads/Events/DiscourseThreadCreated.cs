using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.CommunityDiscourse.Domain.DiscourseThreads.Events;

/// <summary>
/// Domain event emitted when a new discourse thread is created.
/// </summary>
/// <param name="AggregateId">The unique identifier of the discourse thread.</param>
/// <param name="Version">The version of the aggregate when this event was raised.</param>
/// <param name="EntityType">The type of entity this discourse thread is about (e.g., "idea", "signal", "action").</param>
/// <param name="EntityId">The identifier of the entity this discourse thread is about.</param>
public sealed record DiscourseThreadCreated(
    string AggregateId,
    long Version,
    string EntityType,
    string EntityId
) : DomainEvent(AggregateId, Version);