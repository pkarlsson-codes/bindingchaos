using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;
using Marten;
using Marten.Events.Projections;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects inquiry lifecycle events into <see cref="ProjectContestationStatusView"/>.
/// Increments open inquiry count when an inquiry is raised; decrements when resolved or lapsed.
/// </summary>
internal sealed class ProjectContestationStatusViewProjection : EventProjection
{
    /// <summary>
    /// Increments the open inquiry count when a new inquiry is raised.
    /// </summary>
    /// <param name="e">The raised event.</param>
    /// <param name="ops">Document operations.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Project(ProjectInquiryRaised e, IDocumentOperations ops)
    {
        var view = await ops.LoadAsync<ProjectContestationStatusView>(e.ProjectId).ConfigureAwait(false)
            ?? new ProjectContestationStatusView { Id = e.ProjectId };
        view.OpenInquiryCount++;
        ops.Store(view);
    }

    /// <summary>
    /// Decrements the open inquiry count when an inquiry is resolved.
    /// </summary>
    /// <param name="e">The resolved event.</param>
    /// <param name="ops">Document operations.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Project(ProjectInquiryResolved e, IDocumentOperations ops)
    {
        await DecrementOpenAsync(e.ProjectId, ops).ConfigureAwait(false);
    }

    /// <summary>
    /// Decrements the open inquiry count when an inquiry lapses.
    /// </summary>
    /// <param name="e">The lapsed event.</param>
    /// <param name="ops">Document operations.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Project(ProjectInquiryLapsed e, IDocumentOperations ops)
    {
        await DecrementOpenAsync(e.ProjectId, ops).ConfigureAwait(false);
    }

    private static async Task DecrementOpenAsync(string projectId, IDocumentOperations ops)
    {
        var view = await ops.LoadAsync<ProjectContestationStatusView>(projectId).ConfigureAwait(false);
        if (view is null)
        {
            return;
        }

        view.OpenInquiryCount = Math.Max(0, view.OpenInquiryCount - 1);
        ops.Store(view);
    }
}
