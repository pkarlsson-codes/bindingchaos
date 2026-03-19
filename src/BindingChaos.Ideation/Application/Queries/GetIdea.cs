using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Ideas;
using Marten;

namespace BindingChaos.Ideation.Application.Queries;

/// <summary>
/// Query to retrieve a specific idea.
/// </summary>
public sealed record GetIdea(IdeaId IdeaId);

/// <summary>Handles the <see cref="GetIdea"/> query.</summary>
public static class GetIdeaHandler
{
    /// <summary>Returns the idea read model for the given ID, or <see langword="null"/> if not found.</summary>
    /// <param name="request">The query containing the idea ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The idea view, or <see langword="null"/> if not found.</returns>
    public static Task<IdeaView?> Handle(GetIdea request, IQuerySession querySession, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return querySession.Query<IdeaView>()
            .Where(idea => idea.Id == request.IdeaId.Value)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
