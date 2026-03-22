using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when an amendment is rejected.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="TargetIdeaId">The target idea ID.</param>
public sealed record AmendmentRejected(
    string AggregateId,
    string TargetIdeaId
) : DomainEvent(AggregateId);
