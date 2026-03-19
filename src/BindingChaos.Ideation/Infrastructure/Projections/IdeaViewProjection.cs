using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.Ideation.Domain.Ideas.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Ideation.Infrastructure.Projections;

/// <summary>
/// Marten projection that builds IdeaReadModel from Idea events.
/// </summary>
internal class IdeaViewProjection : SingleStreamProjection<IdeaView, string>
{
    /// <summary>
    /// Applies the <see cref="IdeaAuthored"/> event to the view model.
    /// </summary>
    /// <param name="view">The idea view to update.</param>
    /// <param name="e">The event containing the idea data.</param>
    public static void Apply(IdeaView view, IdeaAmended e)
    {
        view.CurrentBody = e.NewBody;
        view.CurrentTitle = e.NewTitle;
        view.Version = e.NewVersionNumber;
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies the <see cref="TagAddedToIdea"/> event to the view model.
    /// </summary>
    /// <param name="view">The idea view to update.</param>
    /// <param name="e">The event containing the tag ID to add.</param>
    public static void Apply(IdeaView view, TagAddedToIdea e)
    {
        if (!view.Tags.Contains(e.TagId))
        {
            view.Tags.Add(e.TagId);
        }
    }

    /// <summary>
    /// Applies the <see cref="TagRemovedFromIdea"/> event to the view model.
    /// </summary>
    /// <param name="view">The idea view to update.</param>
    /// <param name="e">The event containing the tag ID to remove.</param>
    public static void Apply(IdeaView view, TagRemovedFromIdea e)
    {
        if (view.Tags.Contains(e.TagId))
        {
            view.Tags.Remove(e.TagId);
        }
    }

    /// <summary>
    /// Creates the initial read model view from an <see cref="IdeaAuthored"/> event.
    /// </summary>
    /// <param name="e">The idea created event wrapped by Marten.</param>
    /// <returns>The initialized idea view.</returns>
    public static IdeaView Create(IEvent<IdeaAuthored> e)
    {
        return new IdeaView
        {
            Id = e.Data.AggregateId,
            SocietyContext = e.Data.SocietyContext,
            CreatedAt = e.Data.OccurredAt,
            CurrentBody = e.Data.Body,
            CurrentTitle = e.Data.Title,
            AuthorId = e.Data.AuthorId,
            SignalReferenceIds = [.. e.Data.SignalReferences],
            LastUpdatedAt = e.Data.OccurredAt,
            ParentIdeaId = e.Data.ParentIdeaId,
            Status = IdeaStatus.Published.Value,
            Tags = [],
            Version = e.Version,
        };
    }

    /// <summary>
    /// Creates a new instance of <see cref="IdeaView"/> based on the specified <see cref="IdeaForked"/> event.
    /// </summary>
    /// <param name="e">The <see cref="IdeaForked"/> event containing the data used to initialize the <see cref="IdeaView"/> instance.</param>
    /// <returns>A new <see cref="IdeaView"/> instance populated with the data from the provided <see cref="IdeaForked"/> event.</returns>
    public static IdeaView Create(IdeaForked e)
    {
        return new IdeaView
        {
            Id = e.AggregateId,
            SocietyContext = e.SocietyContext,
            CreatedAt = e.OccurredAt,
            CurrentBody = e.Body,
            CurrentTitle = e.Title,
            AuthorId = e.AuthorId,
            SignalReferenceIds = [],
            LastUpdatedAt = e.OccurredAt,
            ParentIdeaId = e.ParentIdeaId,
            Status = IdeaStatus.Published.Value,
            Tags = [],
            Version = 1,
        };
    }
}