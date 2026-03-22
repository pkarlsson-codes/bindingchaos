using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a society's description is updated.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="NewDescription">The new description of the society.</param>
public sealed record SocietyDescriptionUpdated(
    string AggregateId,
    string NewDescription
) : DomainEvent(AggregateId);
