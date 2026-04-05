using BindingChaos.SharedKernel.Domain;
using BindingChaos.Societies.Application.ReadModels;
using Marten;

namespace BindingChaos.Societies.Application.Queries;

/// <summary>
/// Query to retrieve a single society by ID.
/// </summary>
/// <param name="SocietyId">The ID of the society to retrieve.</param>
public sealed record GetSociety(SocietyId SocietyId);

/// <summary>
/// Handles the <see cref="GetSociety"/> query.
/// </summary>
public static class GetSocietyHandler
{
    /// <summary>
    /// Returns the society view for the given ID, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="request">The query containing the society ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The society view, or <see langword="null"/> if not found.</returns>
    public static Task<SocietyView?> Handle(
        GetSociety request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<SocietyView>()
            .Where(s => s.Id == request.SocietyId.Value)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
