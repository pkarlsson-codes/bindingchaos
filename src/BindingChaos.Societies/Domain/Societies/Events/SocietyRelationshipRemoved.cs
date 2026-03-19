using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a relationship to another society is removed.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="Version">The aggregate version when this event was raised.</param>
/// <param name="TargetSocietyId">The ID of the target society.</param>
/// <param name="RelationshipType">The name of the relationship type that was removed.</param>
public sealed record SocietyRelationshipRemoved(
    string AggregateId,
    long Version,
    string TargetSocietyId,
    string RelationshipType
) : DomainEvent(AggregateId, Version);
