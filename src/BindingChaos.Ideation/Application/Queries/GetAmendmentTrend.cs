using BindingChaos.Ideation.Application.ReadModels;
using Marten;

namespace BindingChaos.Ideation.Application.Queries;

/// <summary>
/// Query to retrieve support trend data for a specific amendment.
/// </summary>
public sealed record GetAmendmentTrend(string AmendmentId);

/// <summary>Handles the <see cref="GetAmendmentTrend"/> query.</summary>
public static class GetAmendmentTrendHandler
{
    /// <summary>Returns the support trend data for the specified amendment, or <see langword="null"/> if not found.</summary>
    /// <param name="request">The query containing the amendment ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The amendment trend view, or <see langword="null"/> if not found.</returns>
    public static Task<AmendmentTrendView?> Handle(GetAmendmentTrend request, IQuerySession querySession, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<AmendmentTrendView>()
            .Where(trend => trend.AmendmentId == request.AmendmentId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
