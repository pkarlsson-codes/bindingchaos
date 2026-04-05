using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects commons events into <see cref="CommonsListItemView"/>.
/// </summary>
internal sealed class CommonsListItemViewProjection : SingleStreamProjection<CommonsListItemView, string>
{
    /// <summary>
    /// Creates a new <see cref="CommonsListItemView"/> from a <see cref="CommonsCreated"/> event.
    /// </summary>
    /// <param name="e">The created event.</param>
    /// <returns>A new <see cref="CommonsListItemView"/>.</returns>
    public static CommonsListItemView Create(IEvent<CommonsCreated> e) =>
        new()
        {
            Id = e.Data.AggregateId,
            Name = e.Data.Name,
            Description = e.Data.Description,
            Status = CommonsStatus.Proposed.ToString(),
            FounderId = e.Data.FounderId,
            ProposedAt = e.Timestamp,
        };

    /// <summary>
    /// Updates the status to Active on <see cref="CommonsActivated"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The activated event.</param>
    public static void Apply(CommonsListItemView view, CommonsActivated e)
    {
        view.Status = CommonsStatus.Active.ToString();
    }

    /// <summary>
    /// Updates the name on <see cref="CommonsRenamed"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The renamed event.</param>
    public static void Apply(CommonsListItemView view, CommonsRenamed e)
    {
        view.Name = e.NewName;
    }

    /// <summary>
    /// Appends a concern ID to <see cref="CommonsListItemView.LinkedConcernIds"/> when a concern is linked.
    /// </summary>
    /// <param name="e">The concern linked event.</param>
    /// <param name="view">The view to update.</param>
    public static void Apply(IEvent<ConcernLinkedToCommons> e, CommonsListItemView view)
    {
        view.LinkedConcernIds.Add(e.Data.ConcernId);
    }
}
