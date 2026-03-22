using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Ideas.Events;

/// <summary>
/// Domain event raised when a tag is added to an idea.
/// </summary>
/// <param name="AggregateId">The ID of the idea.</param>
/// <param name="UserId">The ID of the user who added the tag.</param>
/// <param name="TagId">The ID of the tag that was added.</param>
public sealed record TagAddedToIdea(
    string AggregateId,
    ParticipantId UserId,
    string TagId
) : DomainEvent(AggregateId);