using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Projects.Events;

/// <summary>
/// Domain event raised when a Contested amendment is rejected after the resolution window.
/// The amendment transitions to Rejected status permanently.
/// </summary>
/// <param name="ProjectId">The identifier of the project.</param>
/// <param name="AmendmentId">The identifier of the rejected amendment.</param>
internal sealed record AmendmentRejected(
    string ProjectId,
    string AmendmentId)
    : DomainEvent(ProjectId);
