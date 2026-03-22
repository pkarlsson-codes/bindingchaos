using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Ideas.Events;

/// <summary>
/// Domain event raised when a tag is removed from an idea.
/// </summary>
/// <param name="AggregateId">The ID of the idea.</param>
/// <param name="UserId">The ID of the user who removed the tag.</param>
/// <param name="TagId">The ID of the tag that was removed.</param>
public sealed record TagRemovedFromIdea(
    string AggregateId,
    ParticipantId UserId,
    string TagId
) : DomainEvent(AggregateId);