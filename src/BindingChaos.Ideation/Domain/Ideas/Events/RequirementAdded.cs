using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Ideas.Events;

/// <summary>
/// Represents an event that records the addition of a requirement to an aggregate within the system.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate to which the requirement is added.</param>
/// <param name="Version">The version of the aggregate at the time the requirement is added.</param>
/// <param name="RequirementId">The unique identifier of the requirement being added.</param>
/// <param name="Label">The descriptive label or name of the requirement.</param>
/// <param name="Quantity">The quantity associated with the requirement. Represents a numeric value that may be fractional.</param>
/// <param name="Unit">The unit of measurement for the quantity, such as 'kg' or 'm'.</param>
/// <param name="Type">The category or type of the requirement, indicating its purpose or classification.</param>
/// <param name="AddedBy">Id of the participant who added the requirement.</param>
public sealed record RequirementAdded(
    string AggregateId,
    long Version,
    string RequirementId,
    string Label,
    double Quantity,
    string Unit,
    int Type,
    string AddedBy
) : DomainEvent(AggregateId, Version);