namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Request model for creating a new idea.
/// </summary>
public sealed class AuthorIdeaRequest
{
    /// <summary>
    /// The title of the idea.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the idea.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The tags associated with the idea.
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// The IDs of the source signals.
    /// </summary>
    public string[] SourceSignalIds { get; set; } = [];

    /// <summary>
    /// The ID of the society context where the idea is being created.
    /// </summary>
    public string SocietyId { get; set; } = string.Empty;
}
