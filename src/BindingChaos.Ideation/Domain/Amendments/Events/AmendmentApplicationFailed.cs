using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Amendments.Events;

/// <summary>
/// Event raised when an attempt to apply an accepted amendment to an idea fails.
/// This can occur due to version mismatches (another amendment was applied first)
/// or if the target idea no longer exists.
/// </summary>
/// <param name="AggregateId">The amendment ID that failed to apply.</param>
/// <param name="Version">The aggregate version when raised.</param>
/// <param name="Reason">The reason for failure (e.g., "VersionMismatch", "IdeaNotFound").</param>
/// <param name="IdeaCurrentVersion">The current version of the idea (if applicable), null if idea not found.</param>
public sealed record AmendmentApplicationFailed(
    string AggregateId,
    long Version,
    string Reason,
    int? IdeaCurrentVersion
) : DomainEvent(AggregateId, Version);
