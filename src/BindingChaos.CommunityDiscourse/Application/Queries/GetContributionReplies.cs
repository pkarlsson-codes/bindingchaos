using BindingChaos.CommunityDiscourse.Application.DTOs;
using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Application.Specifications;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Specifications;
using Marten;

namespace BindingChaos.CommunityDiscourse.Application.Queries;

/// <summary>
/// Query to retrieve replies for a specific contribution.
/// Supports direct replies only (one level deep) with cursor-based pagination.
/// </summary>
public class GetContributionReplies
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetContributionReplies"/> class.
    /// </summary>
    /// <param name="contributionId">The contribution ID to get replies for.</param>
    /// <param name="cursor">The cursor to start from (null for first page).</param>
    /// <param name="limit">Maximum number of replies to return (default: 5).</param>
    /// <param name="direction">Direction of pagination (default: Forward).</param>
    public GetContributionReplies(string contributionId, string? cursor = null, int limit = 5, CursorDirection direction = CursorDirection.Forward)
    {
        ContributionId = contributionId ?? throw new ArgumentNullException(nameof(contributionId));
        Cursor = cursor;
        Limit = limit;
        Direction = direction;

        if (limit <= 0)
        {
            throw new ArgumentException("Limit must be positive", nameof(limit));
        }

        if (!string.IsNullOrEmpty(cursor) && !CursorHelper.IsValidCursor(cursor))
        {
            throw new ArgumentException("Invalid cursor format", nameof(cursor));
        }
    }

    /// <summary>
    /// Gets the contribution ID to get replies for.
    /// </summary>
    public string ContributionId { get; }

    /// <summary>
    /// Gets the cursor to start from (null for first page).
    /// </summary>
    public string? Cursor { get; }

    /// <summary>
    /// Gets the maximum number of replies to return.
    /// </summary>
    public int Limit { get; }

    /// <summary>
    /// Gets the direction of pagination.
    /// </summary>
    public CursorDirection Direction { get; }
}

/// <summary>
/// Efficient query handler for contribution replies using cursor-based pagination.
/// Achieves TRUE database-level pagination by querying individual contribution documents with cursors.
/// </summary>
public static class GetContributionRepliesHandler
{
    /// <summary>
    /// Retrieves replies to a specific contribution based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing the contribution ID, pagination details, and direction.</param>
    /// <param name="querySession">The read-only query session for accessing the read model.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ContributionRepliesView"/> with the retrieved replies, or <see langword="null"/> if the contribution is not found.</returns>
    public static async Task<ContributionRepliesView?> Handle(
        GetContributionReplies request,
        IQuerySession querySession,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var parentContribution = await querySession.Query<ContributionView>()
            .Where(c => c.Id == request.ContributionId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (parentContribution == null)
        {
            return null;
        }

        var specification = ContributionCursorHelpers.BuildContributionSpecification(
            new RepliesForContributionSpecification(request.ContributionId), request.Cursor, request.Direction);

        var baseQuery = querySession.Query<ContributionView>().Matching(specification);
        var orderedQuery = ContributionCursorHelpers.ApplyDirectionalOrdering(baseQuery, request.Direction);
        var (replyDocuments, hasMore) = await ContributionCursorHelpers.ExecuteCursorPageAsync(
            orderedQuery, request.Limit, request.Direction, cancellationToken).ConfigureAwait(false);

        var replies = replyDocuments.Select(doc => new ContributionReply
        {
            Id = doc.Id,
            AuthorId = doc.AuthorId,
            Content = doc.Content,
            CreatedAt = doc.CreatedAt,
            ParentContributionId = doc.ParentContributionId,
            Replies = [],
            ReplyCount = doc.ReplyCount,
            HasMoreReplies = doc.ReplyCount > 0,
        }).ToArray();

        var nextCursor = hasMore && request.Direction == CursorDirection.Forward
            ? CursorHelper.CreateNextCursor(replies, r => r.CreatedAt, r => r.Id)
            : null;

        var previousCursor = hasMore && request.Direction == CursorDirection.Backward
            ? CursorHelper.CreatePreviousCursor(replies, r => r.CreatedAt, r => r.Id)
            : null;

        return new ContributionRepliesView
        {
            ContributionId = request.ContributionId,
            Replies = new CursorPaginatedResponse<ContributionReply>
            {
                Items = replies,
                NextCursor = nextCursor,
                PreviousCursor = previousCursor,
                PageSize = request.Limit,
            },
        };
    }
}
