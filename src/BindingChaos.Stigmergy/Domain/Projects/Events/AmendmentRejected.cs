using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Projects.Events;

/// <summary>
/// Domain event raised when a Contested amendment is rejected after the resolution window.
/// The amendment transitions to Rejected status permanently.
/// </summary>
/// <param name="ProjectId">The identifier of the project.</param>
/// <param name="Version">The version of the project at the time of rejection.</param>
/// <param name="AmendmentId">The identifier of the rejected amendment.</param>
internal sealed record AmendmentRejected(
    string ProjectId,
    long Version,
    string AmendmentId)
    : DomainEvent(ProjectId, Version);
