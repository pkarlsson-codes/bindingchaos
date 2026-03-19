namespace BindingChaos.Societies.Application.ReadModels;

/// <summary>
/// Read model for a society in a paginated list.
/// </summary>
public class SocietyListItemView
{
    /// <summary>
    /// Gets or sets the unique identifier of the society.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the society.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the society.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when this society was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with this society.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether this society has geographic bounds.
    /// </summary>
    public bool HasGeographicBounds { get; set; }

    /// <summary>
    /// Gets or sets the number of active members.
    /// </summary>
    public int ActiveMemberCount { get; set; }
}
