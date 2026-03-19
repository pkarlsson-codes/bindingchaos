using BindingChaos.SignalAwareness.Application.ReadModels;
using Marten;

namespace BindingChaos.SignalAwareness.Application.Queries;

/// <summary>
/// Query to retrieve amplification trend data for a specific signal.
/// </summary>
public record GetSignalAmplificationTrend(string SignalId);

/// <summary>Handles the <see cref="GetSignalAmplificationTrend"/> query.</summary>
public static class GetSignalAmplificationTrendHandler
{
    /// <summary>Returns the amplification trend data for the specified signal, or <see langword="null"/> if not found.</summary>
    /// <param name="request">The query containing the signal ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The signal amplification trend view, or <see langword="null"/> if not found.</returns>
    public static Task<SignalAmplificationTrendView?> Handle(
        GetSignalAmplificationTrend request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<SignalAmplificationTrendView>()
            .Where(trend => trend.SignalId == request.SignalId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
