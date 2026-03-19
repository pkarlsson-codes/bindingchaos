using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Societies.Application.ReadModels;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Societies.Application.Queries;

/// <summary>
/// Query to retrieve a paginated list of societies with optional filtering.
/// </summary>
/// <param name="QuerySpec">The pagination and filter specification.</param>
public sealed record GetSocieties(PaginationQuerySpec<SocietiesQueryFilter> QuerySpec);

/// <summary>
/// Handles the <see cref="GetSocieties"/> query.
/// </summary>
public static class GetSocietiesHandler
{
    /// <summary>
    /// Returns a paginated list of societies matching the query criteria.
    /// </summary>
    /// <param name="request">The query containing pagination and filter specification.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of society list items.</returns>
    public static async Task<PaginatedResponse<SocietyListItemView>> Handle(
        GetSocieties request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = querySession.Query<SocietyListItemView>();

        IQueryable<SocietyListItemView> filtered = query;

        if (!string.IsNullOrWhiteSpace(request.QuerySpec.Filter.Tag))
        {
            filtered = filtered.Where(s => s.Tags.Contains(request.QuerySpec.Filter.Tag));
        }

        if (request.QuerySpec.Filter.HasGeographicBounds.HasValue)
        {
            var wantGeo = request.QuerySpec.Filter.HasGeographicBounds.Value;
            filtered = filtered.Where(s => s.HasGeographicBounds == wantGeo);
        }

        var page = await filtered
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<SocietyListItemView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
