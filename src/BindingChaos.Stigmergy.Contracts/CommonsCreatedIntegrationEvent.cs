using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Integration event published when a Commons is proposed in the Stigmergy bounded context.
/// </summary>
/// <param name="CommonsId">The unique identifier of the proposed commons.</param>
/// <param name="Name">The name of the commons.</param>
/// <param name="Description">The description of the commons.</param>
/// <param name="FounderId">The identifier of the participant who proposed the commons.</param>
/// <param name="ProposedAt">When the commons was proposed.</param>
public sealed record CommonsCreatedIntegrationEvent(
    string CommonsId,
    string Name,
    string Description,
    string FounderId,
    DateTimeOffset ProposedAt
) : IntegrationEvent;
