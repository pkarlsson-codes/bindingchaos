using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Domain.Contributions.Events;
using Marten.Events.Projections;

namespace BindingChaos.CommunityDiscourse.Infrastructure.Projections;

/// <summary>
/// Multi-stream projection for Contribution aggregate.
/// Builds contribution documents and updates parent reply counts when replies are posted.
/// </summary>
public class ContributionViewProjection : MultiStreamProjection<ContributionView, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContributionViewProjection"/> class, configuring event mappings for
    /// contributions.
    /// </summary>
    public ContributionViewProjection()
    {
        Identities<ContributionAdded>(e =>
        {
            var identities = new List<string> { e.AggregateId };

            if (!string.IsNullOrEmpty(e.ParentContributionId))
            {
                identities.Add(e.ParentContributionId);
            }

            return identities;
        });
    }

    /// <summary>
    /// Creates a new instance of <see cref="ContributionView"/> based on the provided <see
    /// cref="ContributionAdded"/> data.
    /// </summary>
    /// <param name="posted">The <see cref="ContributionAdded"/> object containing the data used to initialize the new <see
    /// cref="ContributionView"/>.</param>
    /// <returns>A new <see cref="ContributionView"/> initialized with the values from the <paramref name="posted"/> object.</returns>
    public static ContributionView Create(ContributionAdded posted)
    {
        return new ContributionView
        {
            Id = posted.AggregateId,
            ThreadId = posted.ThreadId,
            AuthorId = posted.AuthorId,
            Content = posted.Content,
            CreatedAt = posted.OccurredAt,
            ParentContributionId = posted.ParentContributionId,
            IsRootContribution = string.IsNullOrEmpty(posted.ParentContributionId),
            ReplyCount = 0,
            LastReplyAt = null,
        };
    }

    /// <summary>
    /// Updates a parent contribution's reply count when a reply is posted.
    /// This method is called for parent contribution documents when a reply is created.
    /// </summary>
    /// <param name="document">The parent contribution document to update.</param>
    /// <param name="posted">The reply contribution event.</param>
    /// <returns>The updated parent contribution document.</returns>
    public static ContributionView Apply(ContributionView document, ContributionAdded posted)
    {
        document.ReplyCount++;
        document.LastReplyAt = posted.OccurredAt;
        return document;
    }
}
