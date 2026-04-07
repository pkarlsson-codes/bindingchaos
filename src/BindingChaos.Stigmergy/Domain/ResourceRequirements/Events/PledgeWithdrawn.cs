using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements.Events;

/// <summary>
/// Domain event raised when a pledge is withdrawn from a resource requirement.
/// </summary>
/// <param name="AggregateId">The id of the resource requirement (stream key).</param>
/// <param name="PledgeId">The id of the pledge being withdrawn.</param>
public sealed record PledgeWithdrawn(
    string AggregateId,
    string PledgeId
) : DomainEvent(AggregateId);
