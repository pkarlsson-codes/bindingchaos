using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when a participant supports an amendment.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="Version">The aggregate version when raised.</param>
/// <param name="SupporterId">The user ID of the supporter.</param>
/// <param name="Reason">The reason provided by the supporter for supporting the amendment.</param>
public sealed record AmendmentSupportAdded(
    string AggregateId,
    long Version,
    string SupporterId,
    string Reason
) : DomainEvent(AggregateId, Version);
