using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Ideas.Events;

/// <summary>
/// Idea drafted event.
/// </summary>
/// <param name="AggregateId">Id of drafted idea.</param>
/// <param name="AuthorId">Id of draft author.</param>
/// <param name="Title">Title of draft.</param>
/// <param name="Description">Description of draft.</param>
public sealed record IdeaDrafted(
    string AggregateId,
    string AuthorId,
    string Title,
    string Description
) : DomainEvent(AggregateId);