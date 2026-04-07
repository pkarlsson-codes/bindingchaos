using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Signals.Events;
using Marten;
using Marten.Events.Projections;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projection for the <see cref="SignalAmplificationsView"/> read model.
/// Creates one document per amplification to support paged queries.
/// </summary>
internal sealed class SignalAmplificationsViewProjection : EventProjection
{
    /// <summary>
    /// Creates a new <see cref="SignalAmplificationsView"/> when a signal is amplified.
    /// </summary>
    /// <param name="e">The event that triggered the creation.</param>
    /// <returns>A new <see cref="SignalAmplificationsView"/> instance.</returns>
    public static SignalAmplificationsView Create(SignalAmplified e) => new()
    {
        Id = $"{e.AggregateId}:{e.AmplifiedById}",
        SignalId = e.AggregateId,
        ActorId = e.AmplifiedById,
        OccurredAt = e.OccurredAt,
    };

    /// <summary>
    /// Deletes the <see cref="SignalAmplificationsView"/> when an amplification is withdrawn.
    /// </summary>
    /// <param name="e">The event that triggered the deletion.</param>
    /// <param name="ops">The document operations.</param>
    public static void Project(SignalAmplificationWithdrawn e, IDocumentOperations ops)
        => ops.Delete<SignalAmplificationsView>($"{e.AggregateId}:{e.AmplifierId}");
}
