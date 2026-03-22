using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Ideas.Events;

/// <summary>
/// Domain event raised when an idea is created.
/// </summary>
/// <param name="AggregateId">The ID of the created idea.</param>
/// <param name="AuthorId">The ID of the creator.</param>
/// <param name="SocietyContext">The society context (governance jurisdiction) of the idea.</param>
/// <param name="Title">The title of the idea.</param>
/// <param name="Body">The body content of the idea.</param>
/// <param name="SignalReferences">The signal references associated with the idea.</param>
/// <param name="Tags">The tags associated with the idea.</param>
/// <param name="ParentIdeaId">The parent idea ID if the idea was forked from another idea.</param>
public sealed record IdeaAuthored(
    string AggregateId,
    string AuthorId,
    string SocietyContext,
    string Title,
    string Body,
    IReadOnlyList<string> SignalReferences,
    IReadOnlyList<string> Tags,
    string? ParentIdeaId
) : DomainEvent(AggregateId);
