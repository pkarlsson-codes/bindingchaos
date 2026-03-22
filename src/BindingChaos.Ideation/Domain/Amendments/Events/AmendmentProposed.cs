using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when an amendment is proposed.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="TargetIdeaId">The target idea ID.</param>
/// <param name="TargetVersionNumber">The target idea version number.</param>
/// <param name="CreatorId">The creator user ID.</param>
/// <param name="ProposedTitle">The proposed title.</param>
/// <param name="ProposedBody">The proposed body.</param>
/// <param name="AmendmentTitle">The amendment title.</param>
/// <param name="AmendmentDescription">The amendment description.</param>
public sealed record AmendmentProposed(
    string AggregateId,
    string TargetIdeaId,
    int TargetVersionNumber,
    string CreatorId,
    string ProposedTitle,
    string ProposedBody,
    string AmendmentTitle,
    string AmendmentDescription
) : DomainEvent(AggregateId);
