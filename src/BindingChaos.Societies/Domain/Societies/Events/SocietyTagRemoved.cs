using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a tag is removed from a society.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="Tag">The tag that was removed.</param>
public sealed record SocietyTagRemoved(
    string AggregateId,
    string Tag
) : DomainEvent(AggregateId);
