using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements.Events;

/// <summary>
/// Domain event raised when resources are pledged toward a resource requirement.
/// </summary>
/// <param name="AggregateId">The id of the resource requirement (stream key).</param>
/// <param name="PledgeId">The id of the newly created pledge.</param>
/// <param name="PledgedById">The id of the participant making the pledge.</param>
/// <param name="Amount">The amount of resources pledged.</param>
public sealed record ResourcesPledged(
    string AggregateId,
    string PledgeId,
    string PledgedById,
    double Amount
) : DomainEvent(AggregateId);
