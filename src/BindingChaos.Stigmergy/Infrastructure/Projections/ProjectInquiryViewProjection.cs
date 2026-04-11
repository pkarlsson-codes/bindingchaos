using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>Projects <see cref="Domain.ProjectInquiries.ProjectInquiry"/> events into <see cref="ProjectInquiryView"/>.</summary>
internal sealed class ProjectInquiryViewProjection : SingleStreamProjection<ProjectInquiryView, string>
{
    /// <summary>Creates a new view from the raised event.</summary>
    /// <param name="e">The raised event.</param>
    /// <returns>A new <see cref="ProjectInquiryView"/>.</returns>
    public static ProjectInquiryView Create(IEvent<ProjectInquiryRaised> e) => new()
    {
        Id = e.Data.AggregateId,
        ProjectId = e.Data.ProjectId,
        RaisedByParticipantId = e.Data.RaisedByParticipantId,
        RaisedBySocietyId = e.Data.RaisedBySocietyId,
        Body = e.Data.Body,
        Status = ProjectInquiryStatus.Open.ToString(),
        RaisedAt = e.Timestamp,
        LastUpdatedAt = e.Timestamp,
    };

    /// <summary>Updates view on responded event.</summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The responded event.</param>
    public static void Apply(ProjectInquiryView view, IEvent<ProjectInquiryResponded> e)
    {
        view.Response = e.Data.Response;
        view.Status = ProjectInquiryStatus.Responded.ToString();
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>Updates view on resolved event.</summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The resolved event.</param>
    public static void Apply(ProjectInquiryView view, IEvent<ProjectInquiryResolved> e)
    {
        view.Status = ProjectInquiryStatus.Resolved.ToString();
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>Updates view on updated event.</summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The updated event.</param>
    public static void Apply(ProjectInquiryView view, IEvent<ProjectInquiryUpdated> e)
    {
        view.Body = e.Data.NewBody;
        view.Response = null;
        view.Status = ProjectInquiryStatus.Open.ToString();
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>Updates view on lapsed event.</summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The lapsed event.</param>
    public static void Apply(ProjectInquiryView view, IEvent<ProjectInquiryLapsed> e)
    {
        view.Status = ProjectInquiryStatus.Lapsed.ToString();
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>Updates view on reopened event.</summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The reopened event.</param>
    public static void Apply(ProjectInquiryView view, IEvent<ProjectInquiryReopened> e)
    {
        if (e.Data.UpdatedBody is not null)
        {
            view.Body = e.Data.UpdatedBody;
        }

        view.Response = null;
        view.Status = ProjectInquiryStatus.Open.ToString();
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>Updates view when discourse thread is linked.</summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The discourse thread linked event.</param>
    public static void Apply(ProjectInquiryView view, IEvent<ProjectInquiryDiscourseThreadLinked> e)
    {
        view.DiscourseThreadId = e.Data.DiscourseThreadId;
        view.LastUpdatedAt = e.Timestamp;
    }
}
