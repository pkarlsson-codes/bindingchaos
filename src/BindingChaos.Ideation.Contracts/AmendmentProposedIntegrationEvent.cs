using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Contracts;

/// <summary>
/// Integration event published when an amendment is proposed in the Ideation bounded context.
/// This event is used to notify other bounded contexts about amendment creation without creating direct dependencies.
/// </summary>
/// <param name="AmendmentId">The unique identifier of the proposed amendment.</param>
/// <param name="TargetIdeaId">The ID of the target idea being amended.</param>
/// <param name="CreatorId">The ID of the creator.</param>
/// <param name="ProposedTitle">The proposed title for the amendment.</param>
/// <param name="ProposedBody">The proposed body content for the amendment.</param>
/// <param name="AmendmentTitle">The title of the amendment.</param>
/// <param name="AmendmentDescription">The description of the amendment.</param>
/// <param name="ProposedAt">When the amendment was proposed.</param>
public sealed record AmendmentProposedIntegrationEvent(
    string AmendmentId,
    string TargetIdeaId,
    string CreatorId,
    string ProposedTitle,
    string ProposedBody,
    string AmendmentTitle,
    string AmendmentDescription,
    DateTimeOffset ProposedAt
) : IntegrationEvent;
