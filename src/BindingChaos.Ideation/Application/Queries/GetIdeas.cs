using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Ideation.Application.Queries;

/// <summary>
/// Represents a query for retrieving ideas with optional filtering and pagination.
/// </summary>
/// <param name="QuerySpec">The specification for pagination and filtering of the ideas query.</param>
public sealed record GetIdeas(PaginationQuerySpec<IdeasQueryFilter> QuerySpec);

/// <summary>Handles <see cref="GetIdeas"/> requests by querying the read database.</summary>
public static class GetIdeasHandler
{
    /// <summary>Returns a paginated list of ideas matching the query criteria.</summary>
    /// <param name="request">The query containing the pagination and filter specification.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of ideas matching the query criteria.</returns>
    public static async Task<PaginatedResponse<IdeasListItemView>> Handle(
        GetIdeas request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var societyIds = (request.QuerySpec.Filter.SocietyIds ?? [])
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToArray();

        var baseQuery = querySession.Query<IdeasListItemView>();

        var filteredQuery = societyIds.Length > 0
            ? baseQuery.Where(idea => societyIds.Contains(idea.SocietyContext))
            : baseQuery;

        var page = await filteredQuery
            .Sort(request.QuerySpec.SortDescriptors, IdeasListItemView.SortMappings)
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<IdeasListItemView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
