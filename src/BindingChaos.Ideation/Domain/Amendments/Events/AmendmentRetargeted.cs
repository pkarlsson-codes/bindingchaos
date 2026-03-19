using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when an outdated amendment is retargeted to a newer idea version.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="Version">The aggregate version when raised.</param>
/// <param name="TargetIdeaId">The target idea ID.</param>
/// <param name="OldTargetVersionNumber">The previously targeted version number.</param>
/// <param name="NewTargetVersionNumber">The new targeted version number.</param>
/// <param name="NewTitle">Optional updated title.</param>
/// <param name="NewBody">Optional updated body.</param>
/// <param name="NewAmendmentTitle">Optional updated amendment title.</param>
/// <param name="NewAmendmentDescription">Optional updated amendment description.</param>
public sealed record AmendmentRetargeted(
    string AggregateId,
    long Version,
    string TargetIdeaId,
    int OldTargetVersionNumber,
    int NewTargetVersionNumber,
    string? NewTitle,
    string? NewBody,
    string? NewAmendmentTitle,
    string? NewAmendmentDescription
) : DomainEvent(AggregateId, Version);
