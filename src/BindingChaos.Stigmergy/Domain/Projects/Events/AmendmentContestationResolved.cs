using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Projects.Events;

/// <summary>
/// Domain event raised when a Contested amendment's resolution window expires and
/// the amendment was not rejected — it returns to Active status.
/// </summary>
/// <param name="ProjectId">The identifier of the project.</param>
/// <param name="AmendmentId">The identifier of the amendment being restored to Active.</param>
internal sealed record AmendmentContestationResolved(
    string ProjectId,
    string AmendmentId)
    : DomainEvent(ProjectId);
