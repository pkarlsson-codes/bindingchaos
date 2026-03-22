using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Projects.Events;

/// <summary>
/// Domain event raised when an Active amendment is contested by a group member.
/// The amendment transitions to Contested status and is no longer in effect.
/// </summary>
/// <param name="ProjectId">The identifier of the project.</param>
/// <param name="Version">The version of the project at the time of contention.</param>
/// <param name="AmendmentId">The identifier of the contested amendment.</param>
/// <param name="ContesterId">The identifier of the participant who contested the amendment.</param>
/// <param name="ContestedAt">The timestamp when the amendment was contested.</param>
internal sealed record AmendmentContested(
    string ProjectId,
    long Version,
    string AmendmentId,
    string ContesterId,
    DateTimeOffset ContestedAt)
    : DomainEvent(ProjectId, Version);
