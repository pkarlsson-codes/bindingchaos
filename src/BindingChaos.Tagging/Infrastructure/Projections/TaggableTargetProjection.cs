using BindingChaos.Tagging.Application.ReadModels;
using BindingChaos.Tagging.Domain.TaggableTargets.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Tagging.Infrastructure.Projections;

/// <summary>
/// Represents a projection that provides a view of a taggable target's properties.
/// </summary>
internal sealed class TaggableTargetProjection : SingleStreamProjection<TaggableTargetView, string>
{
    /// <summary>
    /// Creates a new instance of the TaggableTargetView class using the specified event data.
    /// </summary>
    /// <param name="e">The event data containing the aggregate identifier and locality identifier used to initialize the
    /// TaggableTargetView.</param>
    /// <returns>A new TaggableTargetView instance initialized with values from the provided event data.</returns>
    public static TaggableTargetView Create(TaggableTargetCreated e)
    {
        return new TaggableTargetView
        {
            Id = e.AggregateId,
        };
    }

    /// <summary>
    /// Applies the specified tags to the target view by updating its tag collection with the provided tag identifiers.
    /// </summary>
    /// <param name="view">The target view whose tag collection will be updated. Cannot be null.</param>
    /// <param name="e">An event containing the tag identifiers to assign to the target view. Cannot be null.</param>
    public static void Apply(TaggableTargetView view, TagsAssigned e)
    {
        if (e.TagIds.Length == 0)
        {
            return;
        }

        view.Tags = AppendTags(view.Tags, e.TagIds);
    }

    /// <summary>
    /// Removes the specified tag identifiers from the collection of tags associated with the provided view.
    /// </summary>
    /// <param name="view">The target view whose tags will be updated. This parameter must not be null.</param>
    /// <param name="e">An event containing the identifiers of the tags to remove from the view's tag collection. This parameter must
    /// not be null.</param>
    public static void Apply(TaggableTargetView view, TagsRemoved e)
    {
        if (view.Tags.Length == 0 || e.TagIds.Length == 0)
        {
            return;
        }

        view.Tags = RemoveTags(view.Tags, e.TagIds);
    }

    private static string[] AppendTags(string[] currentTags, string[] tagsToAdd)
    {
        if (currentTags.Length == 0)
        {
            return [.. tagsToAdd];
        }

        var combinedTags = new string[currentTags.Length + tagsToAdd.Length];
        Array.Copy(currentTags, combinedTags, currentTags.Length);
        Array.Copy(tagsToAdd, 0, combinedTags, currentTags.Length, tagsToAdd.Length);
        return combinedTags;
    }

    private static string[] RemoveTags(string[] currentTags, string[] tagsToRemove)
    {
        var removals = tagsToRemove.ToHashSet(StringComparer.Ordinal);

        var remainingTags = new List<string>(currentTags.Length);
        foreach (var tag in currentTags)
        {
            if (!removals.Contains(tag))
            {
                remainingTags.Add(tag);
            }
        }

        return [.. remainingTags];
    }
}
