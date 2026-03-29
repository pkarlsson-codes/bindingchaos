using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Ideas.Events;

/// <summary>
/// Idea forked event.
/// </summary>
/// <param name="AggregateId">Id of the newly forked <see cref="Idea"/>.</param>
/// <param name="ParentIdeaId">Id of the forked parent <see cref="Idea"/>.</param>
/// <param name="AuthorId">Id of the author that forked the idea.</param>
/// <param name="Title">Title of the forked draft <see cref="Idea"/>.</param>
/// <param name="Description">Description of the forked draft <see cref="Idea"/></param>
public sealed record IdeaForked(
    string AggregateId,
    string ParentIdeaId,
    string AuthorId,
    string Title,
    string Description
) : DomainEvent(AggregateId);