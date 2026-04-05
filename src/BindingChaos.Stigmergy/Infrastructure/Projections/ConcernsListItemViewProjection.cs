using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Concerns.Events;
using Marten;
using Marten.Events.Projections;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projection for <see cref="ConcernsListItemView"/>.
/// </summary>
internal sealed class ConcernsListItemViewProjection : EventProjection
{
    /// <summary>
    /// Creates a new <see cref="ConcernsListItemView"/> from a <see cref="ConcernRaised"/> event,
    /// enriching signal references with titles loaded from <see cref="SignalsListItemView"/>.
    /// </summary>
    /// <param name="e">The <see cref="ConcernRaised"/> event.</param>
    /// <param name="ops">The document operations used to load signal data and store the view.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Project(ConcernRaised e, IDocumentOperations ops)
    {
        var signals = await ops
            .LoadManyAsync<SignalsListItemView>(e.SignalIds.ToArray())
            .ConfigureAwait(false);

        ops.Store(new ConcernsListItemView
        {
            Id = e.AggregateId,
            RaisedById = e.ActorId,
            Name = e.Name,
            Tags = [.. e.Tags],
            Signals = [.. signals
                .Where(s => s is not null)
                .Select(s => new ConcernsListItemView.ReferenceSignal { Id = s.Id, Title = s.Title })],
        });
    }

    /// <summary>
    /// Updates the <see cref="ConcernsListItemView"/> by adding the participant to the list of affected participants.
    /// </summary>
    /// <param name="e">The <see cref="AffectednessIndicated"/> event.</param>
    /// <param name="ops">The document operations used to load and store the view.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Project(AffectednessIndicated e, IDocumentOperations ops)
    {
        var view = await ops.LoadAsync<ConcernsListItemView>(e.AggregateId).ConfigureAwait(false);
        if (view is null)
        {
            return;
        }

        view.AffectedParticipantIds.Add(e.IndicatedById);
        ops.Store(view);
    }

    /// <summary>
    /// Updates the <see cref="ConcernsListItemView"/> by removing the participant from the list of affected participants.
    /// </summary>
    /// <param name="e">The <see cref="AffectednessWithdrawn"/> event.</param>
    /// <param name="ops">The document operations used to load and store the view.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Project(AffectednessWithdrawn e, IDocumentOperations ops)
    {
        var view = await ops.LoadAsync<ConcernsListItemView>(e.AggregateId).ConfigureAwait(false);
        if (view is null)
        {
            return;
        }

        view.AffectedParticipantIds.Remove(e.WithdrawnById);
        ops.Store(view);
    }
}