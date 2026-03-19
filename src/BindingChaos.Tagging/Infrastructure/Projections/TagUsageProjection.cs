using BindingChaos.Tagging.Application.ReadModels;
using BindingChaos.Tagging.Domain.Tags.Events;
using Marten.Events.Projections;

namespace BindingChaos.Tagging.Infrastructure.Projections;

/// <summary>
/// Provides a projection for aggregating and representing tag usage data across multiple streams in a specified view
/// format.
/// </summary>
internal sealed class TagUsageProjection : MultiStreamProjection<TagUsageView, string>
{
    /// <summary>
    /// Initializes a new instance of the TagUsageProjection class, configuring identity mappings for tag-related
    /// events.
    /// </summary>
    public TagUsageProjection()
    {
        Identity<TagCreated>(e => e.AggregateId);
    }

    /// <summary>
    /// Creates a new instance of the TagUsageView class using the data from a TagCreated event.
    /// </summary>
    /// <param name="e">The TagCreated event containing the information used to initialize the TagUsageView instance. Cannot be null.</param>
    /// <returns>A TagUsageView instance populated with values from the specified TagCreated event. The instance will have its
    /// usage count set to zero and its first and last used timestamps set to their default values.</returns>
    public static TagUsageView Create(TagCreated e) => new()
    {
        Id = e.AggregateId,
        Slug = e.PreferredSlug,
        FirstUsedAt = default,
        LastUsedAt = default,
        UsageCount = default,
    };

    /// <summary>
    /// Applies the specified tag creation event to the provided tag usage view, updating its properties to reflect the
    /// new tag information.
    /// </summary>
    /// <param name="view">The tag usage view instance to be updated with the tag creation details.</param>
    /// <param name="e">The tag creation event containing the preferred slug to apply to the tag usage view.</param>
    /// <returns>The updated tag usage view instance with properties set according to the tag creation event.</returns>
    public static TagUsageView Apply(TagUsageView view, TagCreated e)
    {
        view.Slug = e.PreferredSlug;

        return view;
    }
}