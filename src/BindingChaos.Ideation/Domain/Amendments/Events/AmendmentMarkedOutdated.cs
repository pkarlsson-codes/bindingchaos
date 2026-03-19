using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event emitted when an amendment becomes outdated due to a newer idea version being published.
/// </summary>
/// <param name="AggregateId">The amendment ID.</param>
/// <param name="Version">The aggregate version when raised.</param>
/// <param name="TargetIdeaId">The target idea ID.</param>
/// <param name="OldTargetVersionNumber">The previously targeted version number.</param>
/// <param name="NewCurrentVersionNumber">The newly current idea version number.</param>
public sealed record AmendmentMarkedOutdated(
    string AggregateId,
    long Version,
    string TargetIdeaId,
    int OldTargetVersionNumber,
    int NewCurrentVersionNumber
) : DomainEvent(AggregateId, Version);
