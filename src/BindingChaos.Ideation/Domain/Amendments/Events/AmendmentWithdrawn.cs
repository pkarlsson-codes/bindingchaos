using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when an amendment is withdrawn by its creator.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="WithdrawerId">The user ID of the withdrawer.</param>
public sealed record AmendmentWithdrawn(
    string AggregateId,
    string WithdrawerId
) : DomainEvent(AggregateId);
