using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements.Events;

/// <summary>
/// Domain event raised when the quantity needed for a resource requirement is updated.
/// </summary>
/// <param name="AggregateId">The id of the resource requirement (stream key).</param>
/// <param name="QuantityNeeded">The new quantity needed.</param>
public sealed record RequirementQuantityUpdated(
    string AggregateId,
    double QuantityNeeded
) : DomainEvent(AggregateId);
