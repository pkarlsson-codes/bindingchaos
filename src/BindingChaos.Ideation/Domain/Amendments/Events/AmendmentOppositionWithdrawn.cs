using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when a participant withdraws their opposition to an amendment.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="Version">The aggregate version when raised.</param>
/// <param name="OpponentId">The user ID of the withdrawing participant.</param>
public sealed record AmendmentOppositionWithdrawn(
    string AggregateId,
    long Version,
    string OpponentId
) : DomainEvent(AggregateId, Version);
