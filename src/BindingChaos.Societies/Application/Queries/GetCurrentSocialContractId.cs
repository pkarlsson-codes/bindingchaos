using BindingChaos.SharedKernel.Domain;
using BindingChaos.Societies.Application.ReadModels;
using Marten;

namespace BindingChaos.Societies.Application.Queries;

/// <summary>
/// Query to retrieve the current social contract ID for a society.
/// </summary>
/// <param name="SocietyId">The ID of the society.</param>
public sealed record GetCurrentSocialContractId(SocietyId SocietyId);

/// <summary>
/// Handles the <see cref="GetCurrentSocialContractId"/> query.
/// </summary>
public static class GetCurrentSocialContractIdHandler
{
    /// <summary>
    /// Returns the current social contract ID for the given society, or <see langword="null"/> if none exists.
    /// </summary>
    /// <param name="request">The query containing the society ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The social contract ID string, or <see langword="null"/> if not found.</returns>
    public static Task<string?> Handle(
        GetCurrentSocialContractId request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<SocialContractView>()
            .Where(c => c.SocietyId == request.SocietyId.Value)
            .OrderByDescending(c => c.EstablishedAt)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
