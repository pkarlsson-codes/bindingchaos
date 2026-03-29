using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Integration event published when an idea is drafted in the Stigmergy bounded context.
/// </summary>
/// <param name="IdeaId">The unique identifier of the drafted idea.</param>
/// <param name="AuthorId">The identifier of the participant who drafted the idea.</param>
/// <param name="Title">The title of the idea.</param>
/// <param name="Description">The description of the idea.</param>
public sealed record IdeaDraftedIntegrationEvent(
    string IdeaId,
    string AuthorId,
    string Title,
    string Description
) : IntegrationEvent;
