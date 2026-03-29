using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Idea published integration event.
/// </summary>
/// <param name="IdeaId">Id of the published idea.</param>
/// <param name="ActorId">Id of the actor that published the idea.</param>
public sealed record IdeaPublishedIntegrationEvent(
    string IdeaId,
    string ActorId
) : IntegrationEvent;