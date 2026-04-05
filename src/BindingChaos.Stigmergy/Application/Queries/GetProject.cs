using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Projects;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve a project by ID.
/// </summary>
/// <param name="ProjectId">The project identifier.</param>
public sealed record GetProject(ProjectId ProjectId);

/// <summary>
/// Handles the <see cref="GetProject"/> query.
/// </summary>
public static class GetProjectHandler
{
    /// <summary>
    /// Returns the project view for the given ID, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project view, or <see langword="null"/> if not found.</returns>
    public static Task<ProjectView?> Handle(GetProject request, IQuerySession querySession, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return querySession.Query<ProjectView>()
            .Where(p => p.Id == request.ProjectId.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
