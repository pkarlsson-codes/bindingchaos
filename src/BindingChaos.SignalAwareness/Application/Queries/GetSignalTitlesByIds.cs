using BindingChaos.SignalAwareness.Application.ReadModels;
using Marten;

namespace BindingChaos.SignalAwareness.Application.Queries;

/// <summary>
/// Query to retrieve signal names by their identifiers.
/// </summary>
public sealed record GetSignalTitlesByIds(string[] signalIds);

/// <summary>Handles the <see cref="GetSignalTitlesByIds"/> query.</summary>
public static class GetSignalTitlesByIdsHandler
{
    /// <summary>Returns the titles for the given signal IDs.</summary>
    /// <param name="request">The query containing the signal identifiers.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of signal titles matching the provided IDs.</returns>
    public static async Task<IReadOnlyList<SignalTitle>> Handle(
        GetSignalTitlesByIds request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.signalIds.Length == 0)
        {
            return [];
        }

        return await querySession.Query<SignalTitle>()
            .Where(s => request.signalIds.Contains(s.SignalId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
