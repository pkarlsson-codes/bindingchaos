using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Ideas.Events;

/// <summary>
/// Represents an event indicating that an idea has been forked from an existing draft or idea.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate to which this event belongs.</param>
/// <param name="AuthorId">The unique identifier of the author who forked the idea.</param>
/// <param name="SocietyContext">The society context (governance jurisdiction) of the forked idea.</param>
/// <param name="Title">The title of the forked idea.</param>
/// <param name="Body">The body or content of the forked idea.</param>
/// <param name="SignalReferences">A collection of references to signals associated with the forked idea.</param>
/// <param name="Tags">A collection of tags categorizing the forked idea.</param>
/// <param name="ParentIdeaId">The unique identifier of the parent idea from which this idea was forked.</param>
public sealed record IdeaForked(
    string AggregateId,
    string AuthorId,
    string SocietyContext,
    string Title,
    string Body,
    IReadOnlyList<string> SignalReferences,
    IReadOnlyList<string> Tags,
    string? ParentIdeaId
) : DomainEvent(AggregateId);
