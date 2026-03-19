using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when a participant opposes an amendment.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="Version">The aggregate version when raised.</param>
/// <param name="OpponentId">The user ID of the opponent.</param>
/// <param name="Reason">The reason provided by the opponent for opposing the amendment.</param>
public sealed record AmendmentOppositionAdded(
    string AggregateId,
    long Version,
    string OpponentId,
    string Reason
) : DomainEvent(AggregateId, Version);
