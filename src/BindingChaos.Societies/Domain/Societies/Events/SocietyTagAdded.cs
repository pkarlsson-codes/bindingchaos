using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a tag is added to a society.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="Version">The aggregate version when this event was raised.</param>
/// <param name="Tag">The tag that was added.</param>
public sealed record SocietyTagAdded(
    string AggregateId,
    long Version,
    string Tag
) : DomainEvent(AggregateId, Version);
