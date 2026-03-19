using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments.Events;
using Marten.Events.Projections;

namespace BindingChaos.Ideation.Infrastructure.Projections;

/// <summary>
/// Projection for amendment supporter information.
/// Tracks individual supporter records with timestamps from the domain events.
/// </summary>
internal sealed class AmendmentSupporterViewProjection : MultiStreamProjection<AmendmentSupporterView, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmendmentSupporterViewProjection"/> class, configuring event mappings for
    /// amendment supporters.
    /// </summary>
    public AmendmentSupporterViewProjection()
    {
        Identities<AmendmentSupportAdded>(e => [$"{e.AggregateId}-{e.SupporterId}"]);
        Identities<AmendmentSupportWithdrawn>(e => [$"{e.AggregateId}-{e.SupporterId}"]);
        DeleteEvent<AmendmentSupportWithdrawn>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="AmendmentSupporterView"/> from the <see cref="AmendmentSupportAdded"/> event.
    /// </summary>
    /// <param name="e">The event containing the supporter details.</param>
    /// <returns>A new instance of <see cref="AmendmentSupporterView"/>.</returns>
    public static AmendmentSupporterView Create(AmendmentSupportAdded e)
    {
        return new AmendmentSupporterView
        {
            Id = $"{e.AggregateId}-{e.SupporterId}",
            AmendmentId = e.AggregateId,
            ParticipantId = e.SupporterId,
            Reason = e.Reason,
            SupportedAt = e.OccurredAt,
        };
    }
}
