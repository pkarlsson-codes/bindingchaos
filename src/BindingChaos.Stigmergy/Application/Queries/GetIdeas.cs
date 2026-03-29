using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Specifications;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Application.Specifications;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve ideas with optional filtering and pagination.
/// </summary>
public sealed record GetIdeas(PaginationQuerySpec<IdeasQueryFilter> QuerySpec);

/// <summary>Handles the <see cref="GetIdeas"/> query.</summary>
public static class GetIdeasHandler
{
    /// <summary>Returns a paginated list of ideas matching the query criteria.</summary>
    /// <param name="request">The query containing the filter specification.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of ideas matching the query criteria.</returns>
    public static async Task<PaginatedResponse<IdeasListItemView>> Handle(
        GetIdeas request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var filter = request.QuerySpec.Filter;

        var query = querySession.Query<IdeasListItemView>().Matching(
            IdeasMatchingSearchTermSpecification.Optional(filter.SearchTerm)
            .And(IdeasByStatusSpecification.Optional(filter.Status)));

        query = query.Sort(request.QuerySpec.SortDescriptors, IdeasListItemView.SortMappings);

        var page = await query
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
