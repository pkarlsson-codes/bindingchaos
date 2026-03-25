using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Ideation.Application.Queries;

/// <summary>
/// Query to retrieve a paginated list of opponents for a specific amendment.
/// </summary>
public sealed record GetAmendmentOpponents(string AmendmentId, PaginationQuerySpec<object> QuerySpec);

/// <summary>Handles the <see cref="GetAmendmentOpponents"/> query.</summary>
public static class GetAmendmentOpponentsHandler
{
    /// <summary>Returns a paginated list of opponents for the specified amendment.</summary>
    /// <param name="request">The query containing the amendment ID and pagination specification.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of amendment opponents.</returns>
    public static async Task<PaginatedResponse<AmendmentOpponentView>> Handle(
        GetAmendmentOpponents request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = await querySession.Query<AmendmentOpponentView>()
            .Where(opponent => opponent.AmendmentId == request.AmendmentId)
            .OrderByDescending(opponent => opponent.OpposedAt)
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<AmendmentOpponentView>
        {
            Items = [.. page],
            TotalCount = page.TotalItemCount,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
        };
    }
}
