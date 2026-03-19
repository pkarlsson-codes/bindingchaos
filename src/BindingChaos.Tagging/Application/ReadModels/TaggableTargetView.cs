using BindingChaos.Tagging.Domain.TaggableTargets;

namespace BindingChaos.Tagging.Application.ReadModels;

/// <summary>
/// Represents a view of a taggable target and its associated tags.
/// </summary>
public class TaggableTargetView
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the target type for the operation.
    /// </summary>
    public TargetType TargetType { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with the item, allowing for categorization and filtering.
    /// </summary>
    public string[] Tags { get; set; } = [];
}
