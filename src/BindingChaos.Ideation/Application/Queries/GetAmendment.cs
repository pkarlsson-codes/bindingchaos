using BindingChaos.Ideation.Application.ReadModels;
using Marten;

namespace BindingChaos.Ideation.Application.Queries;

/// <summary>
/// Query to retrieve a single amendment by its ID.
/// </summary>
public sealed record GetAmendment(string AmendmentId);

/// <summary>Handles the <see cref="GetAmendment"/> query.</summary>
public static class GetAmendmentHandler
{
    /// <summary>Returns the amendment detail view for the given ID, or <see langword="null"/> if not found.</summary>
    /// <param name="request">The query containing the amendment ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The amendment detail view, or <see langword="null"/> if not found.</returns>
    public static Task<AmendmentDetailView?> Handle(GetAmendment request, IQuerySession querySession, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<AmendmentDetailView>()
            .Where(amendment => amendment.Id == request.AmendmentId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
