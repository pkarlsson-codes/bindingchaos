using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Concerns.Events;
using Marten;
using Marten.Events.Projections;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projection for the <see cref="ConcernAffectedParticipantsView"/> read model.
/// Creates one document per affected participant to support paged queries.
/// </summary>
internal sealed class ConcernAffectedParticipantsViewProjection : EventProjection
{
    /// <summary>
    /// Creates a new <see cref="ConcernAffectedParticipantsView"/> when a participant indicates affectedness.
    /// </summary>
    /// <param name="e">The event representing a participant indicating affectedness.</param>
    /// <returns>A new <see cref="ConcernAffectedParticipantsView"/> instance.</returns>
    public static ConcernAffectedParticipantsView Create(AffectednessIndicated e) => new()
    {
        Id = $"{e.AggregateId}:{e.IndicatedById}",
        ConcernId = e.AggregateId,
        ParticipantId = e.IndicatedById,
        IndicatedAt = e.OccurredAt,
    };

    /// <summary>
    /// Deletes the <see cref="ConcernAffectedParticipantsView"/> when a participant withdraws affectedness.
    /// </summary>
    /// <param name="e">The event representing a participant withdrawing affectedness.</param>
    /// <param name="ops">The document operations.</param>
    public static void Project(AffectednessWithdrawn e, IDocumentOperations ops)
        => ops.Delete<ConcernAffectedParticipantsView>($"{e.AggregateId}:{e.WithdrawnById}");
}
