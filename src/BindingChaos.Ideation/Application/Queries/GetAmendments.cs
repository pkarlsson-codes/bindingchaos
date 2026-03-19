using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Ideation.Application.Queries;

/// <summary>
/// Query to retrieve a paginated list of amendments based on the provided filter and pagination specifications.
/// </summary>
public record GetAmendments(PaginationQuerySpec<AmendmentsQueryFilter> querySpec);

/// <summary>Handles the <see cref="GetAmendments"/> query.</summary>
public static class GetAmendmentsHandler
{
    /// <summary>Returns a paginated list of amendments matching the query criteria.</summary>
    /// <param name="request">The query containing the pagination and filter specification.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of amendments matching the query criteria.</returns>
    public static async Task<PaginatedResponse<AmendmentsListItemView>> Handle(
        GetAmendments request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var ideaId = request.querySpec.Filter?.IdeaId;
        var statusFilter = request.querySpec.Filter?.StatusFilter;

        var query = querySession.Query<AmendmentsListItemView>()
            .Where(x => x.IdeaId == ideaId);

        if (!string.IsNullOrEmpty(statusFilter))
        {
            var allowedStatuses = GetStatusValuesForFilter(statusFilter);
            if (allowedStatuses.Any())
            {
                query = query.Where(x => allowedStatuses.Contains(x.Status));
            }
        }

        var page = await query
            .ToPagedListAsync(request.querySpec.Page.Number, request.querySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<AmendmentsListItemView>
        {
            Items = [.. page],
            TotalCount = page.TotalItemCount,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
        };
    }

    private static IEnumerable<int> GetStatusValuesForFilter(string statusFilter)
    {
        return statusFilter.ToLowerInvariant() switch
        {
            "open" => [AmendmentStatus.Open.Value],
            "approved" => [AmendmentStatus.Approved.Value, AmendmentStatus.Merged.Value],
            "closed" => [AmendmentStatus.Rejected.Value, AmendmentStatus.Withdrawn.Value, AmendmentStatus.Outdated.Value],
            "all" => [],
            _ => [AmendmentStatus.Open.Value],
        };
    }
}
