using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a tag is added to a society.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="Tag">The tag that was added.</param>
public sealed record SocietyTagAdded(
    string AggregateId,
    string Tag
) : DomainEvent(AggregateId);
