using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Signals;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve a signal by its ID.
/// </summary>
public sealed record GetSignal(SignalId SignalId);

/// <summary>Handles the <see cref="GetSignal"/> query.</summary>
public static class GetSignalHandler
{
    /// <summary>Returns the signal view for the given ID, or <see langword="null"/> if not found.</summary>
    /// <param name="request">The query containing the signal ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The signal view, or <see langword="null"/> if not found.</returns>
    public static Task<SignalView?> Handle(GetSignal request, IQuerySession querySession, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<SignalView>()
            .Where(s => s.Id == request.SignalId.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
