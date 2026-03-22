using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.CommunityDiscourse.Domain.Contributions.Events;

/// <summary>
/// Domain event emitted when a new contribution is posted to a discourse thread.
/// </summary>
/// <param name="AggregateId">The unique identifier of the contribution.</param>
/// <param name="ThreadId">The identifier of the thread this contribution belongs to.</param>
/// <param name="AuthorId">The identifier of the participant who authored the contribution.</param>
/// <param name="Content">The content of the contribution.</param>
/// <param name="ParentContributionId">The identifier of the parent contribution, if this is a reply.</param>
public sealed record ContributionAdded(
    string AggregateId,
    string ThreadId,
    string AuthorId,
    string Content,
    string? ParentContributionId
) : DomainEvent(AggregateId);