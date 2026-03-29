using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Ideas;
using BindingChaos.Stigmergy.Domain.Ideas.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects idea events into <see cref="IdeaView"/>.
/// </summary>
internal sealed class IdeaViewProjection : SingleStreamProjection<IdeaView, string>
{
    /// <summary>
    /// Creates a new <see cref="IdeaView"/> from an <see cref="IdeaDrafted"/> event.
    /// </summary>
    /// <param name="e">The drafted event.</param>
    /// <returns>A new <see cref="IdeaView"/>.</returns>
    public static IdeaView Create(IEvent<IdeaDrafted> e) =>
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
    /// Creates a new <see cref="IdeaView"/> from an <see cref="IdeaForked"/> event.
    /// </summary>
    /// <param name="e">The forked event.</param>
    /// <returns>A new <see cref="IdeaView"/>.</returns>
    public static IdeaView Create(IEvent<IdeaForked> e) =>
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
    public static void Apply(IdeaView view, IEvent<IdeaDraftUpdated> e)
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
    public static void Apply(IdeaView view, IEvent<IdeaPublished> e)
    {
        view.Status = IdeaStatus.Published.ToString();
        view.LastUpdatedAt = e.Timestamp;
    }
}
