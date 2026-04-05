using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Projects.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects project events into <see cref="ProjectsListItemView"/>.
/// </summary>
internal sealed class ProjectsListItemViewProjection : SingleStreamProjection<ProjectsListItemView, string>
{
    /// <summary>
    /// Creates a list-item view from a <see cref="ProjectCreated"/> event.
    /// </summary>
    /// <param name="e">The created event.</param>
    /// <returns>A new <see cref="ProjectsListItemView"/>.</returns>
    public static ProjectsListItemView Create(IEvent<ProjectCreated> e) =>
        new()
        {
            Id = e.Data.ProjectId,
            UserGroupId = e.Data.UserGroupId,
            Title = e.Data.Title,
            Description = e.Data.Description,
            CreatedAt = e.Timestamp,
            LastUpdatedAt = e.Timestamp,
            ActiveAmendmentCount = 0,
            ContestedAmendmentCount = 0,
            RejectedAmendmentCount = 0,
        };

    /// <summary>
    /// Increments active amendment count when an amendment is proposed.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The proposed event.</param>
    public static void Apply(ProjectsListItemView view, IEvent<AmendmentProposed> e)
    {
        view.ActiveAmendmentCount++;
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>
    /// Moves an amendment count from Active to Contested.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The contested event.</param>
    public static void Apply(ProjectsListItemView view, IEvent<AmendmentContested> e)
    {
        if (view.ActiveAmendmentCount > 0)
        {
            view.ActiveAmendmentCount--;
        }

        view.ContestedAmendmentCount++;
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>
    /// Moves an amendment count from Contested to Rejected.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The rejected event.</param>
    public static void Apply(ProjectsListItemView view, IEvent<AmendmentRejected> e)
    {
        if (view.ContestedAmendmentCount > 0)
        {
            view.ContestedAmendmentCount--;
        }

        view.RejectedAmendmentCount++;
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>
    /// Moves an amendment count from Contested back to Active.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The resolved event.</param>
    public static void Apply(ProjectsListItemView view, IEvent<AmendmentContestationResolved> e)
    {
        if (view.ContestedAmendmentCount > 0)
        {
            view.ContestedAmendmentCount--;
        }

        view.ActiveAmendmentCount++;
        view.LastUpdatedAt = e.Timestamp;
    }
}
