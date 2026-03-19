using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments.Events;
using Marten.Events.Projections;

namespace BindingChaos.Ideation.Infrastructure.Projections;

/// <summary>
/// Projection for amendment opponent information.
/// Tracks individual opponent records with timestamps from the domain events.
/// </summary>
internal sealed class AmendmentOpponentViewProjection : MultiStreamProjection<AmendmentOpponentView, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmendmentOpponentViewProjection"/> class, configuring event mappings for
    /// amendment opponents.
    /// </summary>
    public AmendmentOpponentViewProjection()
    {
        Identities<AmendmentOppositionAdded>(e => new[] { $"{e.AggregateId}-{e.OpponentId}" });
        Identities<AmendmentOppositionWithdrawn>(e => new[] { $"{e.AggregateId}-{e.OpponentId}" });
        DeleteEvent<AmendmentOppositionWithdrawn>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="AmendmentOpponentView"/> from the <see cref="AmendmentOppositionAdded"/> event.
    /// </summary>
    /// <param name="e">The event containing the opponent details.</param>
    /// <returns>A new instance of <see cref="AmendmentOpponentView"/>.</returns>
    public static AmendmentOpponentView Create(AmendmentOppositionAdded e)
    {
        return new AmendmentOpponentView
        {
            Id = $"{e.AggregateId}-{e.OpponentId}",
            AmendmentId = e.AggregateId,
            ParticipantId = e.OpponentId,
            Reason = e.Reason,
            OpposedAt = e.OccurredAt,
        };
    }
}
