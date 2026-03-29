using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Ideas.Events;

/// <summary>
/// Idea published event.
/// </summary>
/// <param name="AggregateId">Id of the published <see cref="Idea"/>.</param>
/// <param name="PublishedById">Id of the user that published the <see cref="Idea"/>.</param>
public sealed record IdeaPublished(
    string AggregateId,
    string PublishedById
) : DomainEvent(AggregateId);