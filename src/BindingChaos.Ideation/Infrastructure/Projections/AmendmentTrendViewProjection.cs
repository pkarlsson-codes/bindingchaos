using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Ideation.Infrastructure.Projections;

/// <summary>
/// Projection for amendment support trend information over time.
/// Tracks individual vote events over time for frontend aggregation.
/// </summary>
internal sealed class AmendmentTrendViewProjection : SingleStreamProjection<AmendmentTrendView, string>
{
    /// <summary>
    /// Creates a new instance of <see cref="AmendmentTrendView"/> from the <see cref="AmendmentProposed"/> event.
    /// </summary>
    /// <param name="e">The event containing the amendment proposal details.</param>
    /// <returns>A new instance of <see cref="AmendmentTrendView"/>.</returns>
    public static AmendmentTrendView Create(AmendmentProposed e)
    {
        return new AmendmentTrendView
        {
            Id = e.AggregateId,
            AmendmentId = e.AggregateId,
            DataPoints = [], // Start with empty list - no votes yet
        };
    }

    /// <summary>
    /// Applies the <see cref="AmendmentSupportAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentTrendView view, AmendmentSupportAdded e)
    {
        view.DataPoints.Add(new AmendmentTrendPoint
        {
            Date = e.OccurredAt,
            VoteType = "support",
        });
    }

    /// <summary>
    /// Applies the <see cref="AmendmentSupportWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentTrendView view, AmendmentSupportWithdrawn e)
    {
        view.DataPoints.Add(new AmendmentTrendPoint
        {
            Date = e.OccurredAt,
            VoteType = "withdraw_support",
        });
    }

    /// <summary>
    /// Applies the <see cref="AmendmentOppositionAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentTrendView view, AmendmentOppositionAdded e)
    {
        view.DataPoints.Add(new AmendmentTrendPoint
        {
            Date = e.OccurredAt,
            VoteType = "oppose",
        });
    }

    /// <summary>
    /// Applies the <see cref="AmendmentOppositionWithdrawn"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(AmendmentTrendView view, AmendmentOppositionWithdrawn e)
    {
        view.DataPoints.Add(new AmendmentTrendPoint
        {
            Date = e.OccurredAt,
            VoteType = "withdraw_oppose",
        });
    }
}
