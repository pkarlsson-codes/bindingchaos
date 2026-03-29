using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Integration event published when an amendment is proposed against a project in the Stigmergy bounded context.
/// </summary>
/// <param name="AmendmentId">The unique identifier of the proposed amendment.</param>
/// <param name="ProjectId">The identifier of the project being amended.</param>
/// <param name="ProposedBy">The identifier of the participant who proposed the amendment.</param>
/// <param name="ProposedAt">When the amendment was proposed.</param>
public sealed record AmendmentProposedIntegrationEvent(
    string AmendmentId,
    string ProjectId,
    string ProposedBy,
    DateTimeOffset ProposedAt
) : IntegrationEvent;
