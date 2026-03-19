using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Domain.Contributions.Events;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads.Events;
using Marten.Events.Projections;

namespace BindingChaos.CommunityDiscourse.Infrastructure.Projections;

/// <summary>
/// Multi-stream projection for DiscourseThread aggregate.
/// Builds thread metadata and statistics by consuming events from both Thread and Contribution streams.
/// </summary>
public class DiscourseThreadViewProjection : MultiStreamProjection<DiscourseThreadView, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscourseThreadViewProjection"/> class, configuring mappings for thread-related
    /// events.
    /// </summary>
    public DiscourseThreadViewProjection()
    {
        Identity<DiscourseThreadCreated>(e => e.AggregateId);
        Identity<ContributionAdded>(e => e.ThreadId);
    }

    /// <summary>
    /// Creates a new instance of <see cref="DiscourseThreadView"/> based on the specified <see
    /// cref="DiscourseThreadCreated"/> event.
    /// </summary>
    /// <param name="created">The event containing the data required to initialize the <see cref="DiscourseThreadView"/>.  This parameter cannot be
    /// <see langword="null"/>.</param>
    /// <returns>A new <see cref="DiscourseThreadView"/> initialized with the data from the <paramref name="created"/> event.</returns>
    public static DiscourseThreadView Create(DiscourseThreadCreated created)
    {
        return new DiscourseThreadView
        {
            Id = created.AggregateId,
            EntityType = created.EntityType,
            EntityId = created.EntityId,
            CreatedAt = created.OccurredAt,
            TotalContributions = 0,
            TotalRootContributions = 0,
            TotalParticipants = 0,
            LastActivityAt = created.OccurredAt,
            ParticipantIds = [],
        };
    }

    /// <summary>
    /// Updates the state of a <see cref="DiscourseThreadView"/> based on a new contribution.
    /// </summary>
    /// <param name="document">The <see cref="DiscourseThreadView"/> to be updated. This object represents the current state of the thread.</param>
    /// <param name="posted">The <see cref="ContributionAdded"/> containing details of the new contribution to apply.</param>
    /// <returns>The updated <see cref="DiscourseThreadView"/> reflecting the changes made by the new contribution.</returns>
    public static DiscourseThreadView Apply(DiscourseThreadView document, ContributionAdded posted)
    {
        document.TotalContributions++;

        if (string.IsNullOrEmpty(posted.ParentContributionId))
        {
            document.TotalRootContributions++;
        }

        document.LastActivityAt = posted.OccurredAt;
        document.ParticipantIds.Add(posted.AuthorId);
        document.TotalParticipants = document.ParticipantIds.Count;

        return document;
    }
}
