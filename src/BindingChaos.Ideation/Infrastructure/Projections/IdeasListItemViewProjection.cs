using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Ideas.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Ideation.Infrastructure.Projections;

/// <summary>
/// Projection for listing ideas in a locality.
/// </summary>
internal class IdeasListItemViewProjection : SingleStreamProjection<IdeasListItemView, string>
{
    /// <summary>
    /// Applies the <see cref="IdeaAmended"/> event to the view model when an idea is amended.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event containing the new title and body of the idea.</param>
    public static void Apply(IdeasListItemView view, IdeaAmended e)
    {
        view.Title = e.NewTitle;
        view.Body = e.NewBody;
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Creates a new <see cref="IdeasListItemView"/> from the <see cref="IdeaAuthored"/> event.
    /// </summary>
    /// <param name="e">The event containing the idea data.</param>
    /// <returns>A new instance of <see cref="IdeasListItemView"/>.</returns>
    public static IdeasListItemView Create(IdeaAuthored e)
    {
        return new IdeasListItemView
        {
            Id = e.AggregateId,
            Title = e.Title,
            Body = e.Body,
            OpenAmendmentCount = 0,
            SocietyContext = e.SocietyContext,
            CreatedAt = e.OccurredAt,
            LastUpdatedAt = e.OccurredAt,
            Tags = [.. e.Tags],
        };
    }

    /// <summary>
    /// Creates a new instance of <see cref="IdeasListItemView"/> based on the specified <see cref="IdeaForked"/> event.
    /// </summary>
    /// <param name="e">The <see cref="IdeaForked"/> event containing the data used to initialize the <see cref="IdeasListItemView"/>.</param>
    /// <returns>A new <see cref="IdeasListItemView"/> initialized with the properties of the provided <see cref="IdeaForked"/>
    /// event.</returns>
    public static IdeasListItemView Create(IdeaForked e)
    {
        return new IdeasListItemView
        {
            Id = e.AggregateId,
            Title = e.Title,
            Body = e.Body,
            OpenAmendmentCount = 0,
            SocietyContext = e.SocietyContext,
            CreatedAt = e.OccurredAt,
            LastUpdatedAt = e.OccurredAt,
            Tags = [.. e.Tags],
            ParentIdeaId = e.ParentIdeaId,
        };
    }
}
