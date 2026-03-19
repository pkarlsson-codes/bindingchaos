using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Application.Specifications;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Specifications;
using Marten;

namespace BindingChaos.CommunityDiscourse.Application.Queries;

/// <summary>
/// Shared cursor-based pagination helpers for contribution query handlers.
/// </summary>
internal static class ContributionCursorHelpers
{
    /// <summary>
    /// Combines a base specification with a cursor specification for directional pagination.
    /// </summary>
    /// <param name="baseSpecification">The base filter for the query.</param>
    /// <param name="cursor">The encoded cursor string, or <see langword="null"/> for the first page.</param>
    /// <param name="direction">The direction of pagination.</param>
    /// <returns>The combined specification.</returns>
    internal static Specification<ContributionView> BuildContributionSpecification(
        Specification<ContributionView> baseSpecification,
        string? cursor,
        CursorDirection direction)
    {
        if (string.IsNullOrEmpty(cursor))
        {
            return baseSpecification;
        }

        var (cursorTimestamp, cursorId) = CursorHelper.DecodeCursor(cursor);
        Specification<ContributionView> cursorSpecification = direction == CursorDirection.Forward
            ? new ContributionBeforeCursorSpecification(cursorTimestamp, cursorId)
            : new ContributionAfterCursorSpecification(cursorTimestamp, cursorId);

        return baseSpecification.And(cursorSpecification);
    }

    /// <summary>
    /// Applies ordering to a contribution query based on the pagination direction.
    /// </summary>
    /// <param name="query">The base queryable to order.</param>
    /// <param name="direction">The direction of pagination.</param>
    /// <returns>An ordered queryable.</returns>
    internal static IOrderedQueryable<ContributionView> ApplyDirectionalOrdering(IQueryable<ContributionView> query, CursorDirection direction)
    {
        return direction == CursorDirection.Forward
            ? query.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
            : query.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id);
    }

    /// <summary>
    /// Executes a cursor page query, trimming the extra item used to detect more results.
    /// </summary>
    /// <param name="orderedQuery">The ordered query to execute.</param>
    /// <param name="limit">The requested page size.</param>
    /// <param name="direction">The direction of pagination, used to reverse backward pages.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The page of documents and whether more results exist beyond the page.</returns>
    internal static async Task<(List<ContributionView> Documents, bool HasMore)> ExecuteCursorPageAsync(
        IOrderedQueryable<ContributionView> orderedQuery,
        int limit,
        CursorDirection direction,
        CancellationToken cancellationToken)
    {
        var documents = (await orderedQuery
            .Take(limit + 1)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false)).ToList();

        var hasMore = documents.Count > limit;
        if (hasMore)
        {
            documents = [.. documents.Take(limit)];
        }

        if (direction == CursorDirection.Backward)
        {
            documents.Reverse();
        }

        return (documents, hasMore);
    }
}
