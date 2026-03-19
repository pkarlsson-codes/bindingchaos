using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Amendments.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Ideation.Infrastructure.Projections;

/// <summary>
/// Projection for detailed amendment information.
/// </summary>
internal sealed class AmendmentDetailViewProjection : SingleStreamProjection<AmendmentDetailView, string>
{
    /// <summary>
    /// Creates a new instance of <see cref="AmendmentDetailView"/> from the <see cref="AmendmentProposed"/> event.
    /// </summary>
    /// <param name="e">The event containing the amendment proposal details.</param>
    /// <returns>A new instance of <see cref="AmendmentDetailView"/>.</returns>
    public static AmendmentDetailView Create(AmendmentProposed e)
    {
        return new AmendmentDetailView
        {
            Id = e.AggregateId,
            IdeaId = e.TargetIdeaId,
            TargetVersionNumber = e.TargetVersionNumber,
            CreatorId = e.CreatorId,
            AmendmentTitle = e.AmendmentTitle,
            AmendmentDescription = e.AmendmentDescription,
            ProposedTitle = e.ProposedTitle,
            ProposedBody = e.ProposedBody,
            Status = AmendmentStatus.Open.Value,
            CreatedAt = e.OccurredAt,
        };
    }

    /// <summary>
    /// Applies the <see cref="AmendmentSupportAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentSupportAdded e)
    {
        view.SupporterIds.Add(e.SupporterId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentSupportWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentSupportWithdrawn e)
    {
        view.SupporterIds.Remove(e.SupporterId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentOppositionAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentOppositionAdded e)
    {
        view.OpponentIds.Add(e.OpponentId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentOppositionWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentOppositionWithdrawn e)
    {
        view.OpponentIds.Remove(e.OpponentId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentAccepted"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentAccepted e)
    {
        view.Status = AmendmentStatus.Approved.Value;
        view.AcceptedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies the <see cref="AmendmentRejected"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentRejected e)
    {
        view.Status = AmendmentStatus.Rejected.Value;
        view.RejectedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies the <see cref="AmendmentMarkedOutdated"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentMarkedOutdated e)
    {
        view.Status = AmendmentStatus.Outdated.Value;
    }

    /// <summary>
    /// Applies the <see cref="AmendmentWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentDetailView view, AmendmentWithdrawn e)
    {
        view.Status = AmendmentStatus.Withdrawn.Value;
    }
}
