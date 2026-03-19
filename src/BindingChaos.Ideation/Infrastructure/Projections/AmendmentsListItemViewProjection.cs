using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Amendments.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Ideation.Infrastructure.Projections;

/// <summary>
/// Projection for listing minimal amendment information.
/// </summary>
internal sealed class AmendmentsListItemViewProjection : SingleStreamProjection<AmendmentsListItemView, string>
{
    /// <summary>
    /// Creates a new instance of <see cref="AmendmentsListItemView"/> from the <see cref="AmendmentProposed"/> event.
    /// </summary>
    /// <param name="e">The event containing the amendment proposal details.</param>
    /// <returns>A new instance of <see cref="AmendmentsListItemView"/>.</returns>
    public static AmendmentsListItemView Create(AmendmentProposed e)
    {
        return new AmendmentsListItemView
        {
            Id = e.AggregateId,
            IdeaId = e.TargetIdeaId,
            AuthorId = e.CreatorId,
            AmendmentTitle = e.AmendmentTitle,
            AmendmentDescription = e.AmendmentDescription,
            Status = AmendmentStatus.Open.Value,
            CreatedAt = e.OccurredAt,
        };
    }

    /// <summary>
    /// Applies the <see cref="AmendmentSupportAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentSupportAdded e)
    {
        view.SupporterIds.Add(e.SupporterId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentSupportWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentSupportWithdrawn e)
    {
        view.SupporterIds.Remove(e.SupporterId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentOppositionAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentOppositionAdded e)
    {
        view.OpponentIds.Add(e.OpponentId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentOppositionWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentOppositionWithdrawn e)
    {
        view.OpponentIds.Remove(e.OpponentId);
    }

    /// <summary>
    /// Applies the <see cref="AmendmentAccepted"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentAccepted e)
    {
        view.Status = AmendmentStatus.Approved.Value;
    }

    /// <summary>
    /// Applies the <see cref="AmendmentRejected"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentRejected e)
    {
        view.Status = AmendmentStatus.Rejected.Value;
    }

    /// <summary>
    /// Applies the <see cref="AmendmentWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentWithdrawn e)
    {
        view.Status = AmendmentStatus.Withdrawn.Value;
    }

    /// <summary>
    /// Applies the <see cref="AmendmentMarkedOutdated"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentMarkedOutdated e)
    {
        view.Status = AmendmentStatus.Outdated.Value;
    }

    /// <summary>
    /// Applies the <see cref="AmendmentRetargeted"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentsListItemView view, AmendmentRetargeted e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewAmendmentTitle))
        {
            view.AmendmentTitle = e.NewAmendmentTitle;
        }

        if (!string.IsNullOrWhiteSpace(e.NewAmendmentDescription))
        {
            view.AmendmentDescription = e.NewAmendmentDescription;
        }

        view.Status = AmendmentStatus.Open.Value;
    }
}