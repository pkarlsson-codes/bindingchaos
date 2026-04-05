using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.Projects.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects project events into <see cref="ProjectView"/>.
/// </summary>
internal sealed class ProjectViewProjection : SingleStreamProjection<ProjectView, string>
{
    /// <summary>
    /// Creates a new <see cref="ProjectView"/> from a <see cref="ProjectCreated"/> event.
    /// </summary>
    /// <param name="e">The created event.</param>
    /// <returns>A new <see cref="ProjectView"/>.</returns>
    public static ProjectView Create(IEvent<ProjectCreated> e) =>
        new()
        {
            Id = e.Data.ProjectId,
            UserGroupId = e.Data.UserGroupId,
            Title = e.Data.Title,
            Description = e.Data.Description,
            CreatedAt = e.Timestamp,
            LastUpdatedAt = e.Timestamp,
            Amendments = [],
        };

    /// <summary>
    /// Adds a proposed amendment in Active status.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The proposed event.</param>
    public static void Apply(ProjectView view, IEvent<AmendmentProposed> e)
    {
        view.Amendments.Add(new ProjectView.ProjectAmendmentView
        {
            Id = e.Data.AmendmentId,
            ProposedById = e.Data.ProposedBy,
            ProposedAt = e.Data.ProposedAt,
            Status = AmendmentStatus.Active.ToString(),
            LastStatusChangedAt = e.Timestamp,
        });

        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>
    /// Marks an amendment as Contested.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The contested event.</param>
    public static void Apply(ProjectView view, IEvent<AmendmentContested> e)
    {
        var amendment = GetAmendment(view, e.Data.AmendmentId);
        amendment.Status = AmendmentStatus.Contested.ToString();
        amendment.ContestedById = e.Data.ContesterId;
        amendment.ContestedAt = e.Data.ContestedAt;
        amendment.LastStatusChangedAt = e.Timestamp;
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>
    /// Marks an amendment as Rejected.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The rejected event.</param>
    public static void Apply(ProjectView view, IEvent<AmendmentRejected> e)
    {
        var amendment = GetAmendment(view, e.Data.AmendmentId);
        amendment.Status = AmendmentStatus.Rejected.ToString();
        amendment.LastStatusChangedAt = e.Timestamp;
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>
    /// Marks an amendment as Active when contention is resolved.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The resolved event.</param>
    public static void Apply(ProjectView view, IEvent<AmendmentContestationResolved> e)
    {
        var amendment = GetAmendment(view, e.Data.AmendmentId);
        amendment.Status = AmendmentStatus.Active.ToString();
        amendment.LastStatusChangedAt = e.Timestamp;
        view.LastUpdatedAt = e.Timestamp;
    }

    private static ProjectView.ProjectAmendmentView GetAmendment(ProjectView view, string amendmentId)
    {
        return view.Amendments.Single(a => a.Id == amendmentId);
    }
}
