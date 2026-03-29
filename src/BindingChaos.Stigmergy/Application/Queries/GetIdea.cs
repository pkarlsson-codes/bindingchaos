using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Ideas;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve an idea by its ID.
/// </summary>
public sealed record GetIdea(IdeaId IdeaId);

/// <summary>Handles the <see cref="GetIdea"/> query.</summary>
public static class GetIdeaHandler
{
    /// <summary>Returns the idea view for the given ID, or <see langword="null"/> if not found.</summary>
    /// <param name="request">The query containing the idea ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The idea view, or <see langword="null"/> if not found.</returns>
    public static Task<IdeaView?> Handle(GetIdea request, IQuerySession querySession, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<IdeaView>()
            .Where(i => i.Id == request.IdeaId.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
