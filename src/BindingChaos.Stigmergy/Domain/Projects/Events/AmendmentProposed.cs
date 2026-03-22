using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Projects.Events;

/// <summary>
/// Domain event raised when a new amendment is proposed against a project.
/// </summary>
/// <param name="ProjectId">The identifier of the project being amended.</param>
/// <param name="AmendmentId">The identifier of the new amendment.</param>
/// <param name="ProposedBy">The identifier of the participant who proposed the amendment.</param>
/// <param name="ProposedAt">The timestamp of the proposal.</param>
internal sealed record AmendmentProposed(
    string ProjectId,
    string AmendmentId,
    string ProposedBy,
    DateTimeOffset ProposedAt)
    : DomainEvent(ProjectId);
