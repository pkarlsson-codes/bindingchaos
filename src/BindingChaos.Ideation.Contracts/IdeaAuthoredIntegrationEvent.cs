using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Contracts;

/// <summary>
/// Integration event published when an idea is authored in the Ideation bounded context.
/// This event is used to notify other bounded contexts about idea creation without creating direct dependencies.
/// </summary>
/// <param name="IdeaId">The unique identifier of the authored idea.</param>
/// <param name="AuthorId">The ID of the creator.</param>
/// <param name="SocietyContext">The society context (governance jurisdiction) of the idea.</param>
/// <param name="Title">The title of the idea.</param>
/// <param name="Body">The body content of the idea.</param>
/// <param name="SignalReferences">The signal references associated with the idea.</param>
/// <param name="Tags">The tags associated with the idea.</param>
/// <param name="AuthoredAt">When the idea was authored.</param>
/// <param name="ParentIdeaId">The parent idea ID if the idea was forked from another idea.</param>
public sealed record IdeaAuthoredIntegrationEvent(
    string IdeaId,
    string AuthorId,
    string SocietyContext,
    string Title,
    string Body,
    IReadOnlyList<string> SignalReferences,
    IReadOnlyList<string> Tags,
    DateTimeOffset AuthoredAt,
    string? ParentIdeaId = null
) : IntegrationEvent;
