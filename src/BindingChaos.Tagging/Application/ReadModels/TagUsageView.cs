namespace BindingChaos.Tagging.Application.ReadModels;

/// <summary>
/// Meh.
/// </summary>
public class TagUsageView
{
    /// <summary>
    /// Meh.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Meh.
    /// </summary>
    public string Slug { get; set; } = default!;

    /// <summary>
    /// Meh.
    /// </summary>
    public DateTimeOffset FirstUsedAt { get; set; } = default!;

    /// <summary>
    /// Gets or sets the date and time when the object was last accessed.
    /// </summary>
    public DateTimeOffset LastUsedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of times the tag has been used in this locality context.
    /// </summary>
    public int UsageCount { get; set; }
}
