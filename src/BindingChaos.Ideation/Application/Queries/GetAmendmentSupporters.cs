using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Ideation.Application.Queries;

/// <summary>
/// Query to retrieve a paginated list of supporters for a specific amendment.
/// </summary>
public record GetAmendmentSupporters(string AmendmentId, PaginationQuerySpec<object> QuerySpec);

/// <summary>Handles the <see cref="GetAmendmentSupporters"/> query.</summary>
public static class GetAmendmentSupportersHandler
{
    /// <summary>Returns a paginated list of supporters for the specified amendment.</summary>
    /// <param name="request">The query containing the amendment ID and pagination specification.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of amendment supporters.</returns>
    public static async Task<PaginatedResponse<AmendmentSupporterView>> Handle(
        GetAmendmentSupporters request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = await querySession.Query<AmendmentSupporterView>()
            .Where(supporter => supporter.AmendmentId == request.AmendmentId)
            .OrderByDescending(supporter => supporter.SupportedAt)
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<AmendmentSupporterView>
        {
            Items = [.. page],
            TotalCount = page.TotalItemCount,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
        };
    }
}
