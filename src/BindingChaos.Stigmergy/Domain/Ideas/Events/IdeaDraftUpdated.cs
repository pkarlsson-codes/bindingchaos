using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Ideas.Events;

/// <summary>
/// Idea draft updated event.
/// </summary>
/// <param name="AggregateId">Id of the updated <see cref="Idea"/> draft.</param>
/// <param name="ActorId">Id of the actor that is updating the idea.</param>
/// <param name="NewTitle">New title of the draft.</param>
/// <param name="NewDescription">New description of the draft.</param>
public sealed record IdeaDraftUpdated(
    string AggregateId,
    string ActorId,
    string NewTitle,
    string NewDescription
) : DomainEvent(AggregateId);