using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements.Events;

/// <summary>
/// Domain event raised when a new resource requirement is added to a project.
/// </summary>
/// <param name="AggregateId">The id of the resource requirement (stream key).</param>
/// <param name="ProjectId">The id of the owning project.</param>
/// <param name="Description">The description of the requirement.</param>
/// <param name="QuantityNeeded">The quantity of the resource required.</param>
/// <param name="Unit">The unit of measurement (e.g. "hours", "kg").</param>
public sealed record RequirementAdded(
    string AggregateId,
    string ProjectId,
    string Description,
    double QuantityNeeded,
    string Unit
) : DomainEvent(AggregateId);
