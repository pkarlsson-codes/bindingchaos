using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Projects;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve the contestation status of a project.</summary>
/// <param name="ProjectId">The project identifier.</param>
public sealed record GetProjectContestationStatus(ProjectId ProjectId);

/// <summary>Handles the <see cref="GetProjectContestationStatus"/> query.</summary>
public static class GetProjectContestationStatusHandler
{
    /// <summary>
    /// Returns the contestation status view for the given project, defaulting to zero open inquiries.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The contestation status view.</returns>
    public static async Task<ProjectContestationStatusView> Handle(
        GetProjectContestationStatus request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await querySession.Query<ProjectContestationStatusView>()
            .Where(x => x.Id == request.ProjectId.Value)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? new ProjectContestationStatusView { Id = request.ProjectId.Value, OpenInquiryCount = 0 };
    }
}
