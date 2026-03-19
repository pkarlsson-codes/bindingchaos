using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when a participant withdraws their support for an amendment.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="Version">The aggregate version when raised.</param>
/// <param name="SupporterId">The user ID of the withdrawing participant.</param>
public sealed record AmendmentSupportWithdrawn(
    string AggregateId,
    long Version,
    string SupporterId
) : DomainEvent(AggregateId, Version);
