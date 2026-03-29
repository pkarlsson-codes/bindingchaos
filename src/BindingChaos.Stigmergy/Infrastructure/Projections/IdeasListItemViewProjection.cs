using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Ideas;
using BindingChaos.Stigmergy.Domain.Ideas.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects idea events into <see cref="IdeasListItemView"/>.
/// </summary>
internal sealed class IdeasListItemViewProjection : SingleStreamProjection<IdeasListItemView, string>
{
    /// <summary>
    /// Creates a new <see cref="IdeasListItemView"/> from an <see cref="IdeaDrafted"/> event.
    /// </summary>
    /// <param name="e">The drafted event.</param>
    /// <returns>A new <see cref="IdeasListItemView"/>.</returns>
    public static IdeasListItemView Create(IEvent<IdeaDrafted> e) =>
        new()
        {
            Id = e.Data.AggregateId,
            AuthorId = e.Data.AuthorId,
            Title = e.Data.Title,
            Description = e.Data.Description,
            Status = IdeaStatus.Draft.ToString(),
            CreatedAt = e.Timestamp,
            LastUpdatedAt = e.Timestamp,
        };

    /// <summary>
    /// Creates a new <see cref="IdeasListItemView"/> from an <see cref="IdeaForked"/> event.
    /// </summary>
    /// <param name="e">The forked event.</param>
    /// <returns>A new <see cref="IdeasListItemView"/>.</returns>
    public static IdeasListItemView Create(IEvent<IdeaForked> e) =>
        new()
        {
            Id = e.Data.AggregateId,
            AuthorId = e.Data.AuthorId,
            Title = e.Data.Title,
            Description = e.Data.Description,
            Status = IdeaStatus.Draft.ToString(),
            ParentIdeaId = e.Data.ParentIdeaId,
            CreatedAt = e.Timestamp,
            LastUpdatedAt = e.Timestamp,
        };

    /// <summary>
    /// Updates title and description on <see cref="IdeaDraftUpdated"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The updated event.</param>
    public static void Apply(IdeasListItemView view, IEvent<IdeaDraftUpdated> e)
    {
        view.Title = e.Data.NewTitle;
        view.Description = e.Data.NewDescription;
        view.LastUpdatedAt = e.Timestamp;
    }

    /// <summary>
    /// Updates status to Published on <see cref="IdeaPublished"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The published event.</param>
    public static void Apply(IdeasListItemView view, IEvent<IdeaPublished> e)
    {
        view.Status = IdeaStatus.Published.ToString();
        view.LastUpdatedAt = e.Timestamp;
    }
}
