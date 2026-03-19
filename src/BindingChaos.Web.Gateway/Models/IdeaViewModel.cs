namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for ideas tailored to the web frontend.
/// </summary>
public sealed class IdeaViewModel
{
    /// <summary>
    /// The unique identifier of the idea.
    /// </summary>
    public string Id { get; set; } = string.Empty;

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
    /// The creation timestamp in ISO 8601 format.
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// The last update timestamp in ISO 8601 format.
    /// </summary>
    public string UpdatedAt { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the author.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// The society context of the idea.
    /// </summary>
    public string SocietyContext { get; set; } = string.Empty;

    /// <summary>
    /// The status of the idea.
    /// </summary>
    public string Status { get; set; } = "active";

    /// <summary>
    /// The number of contributors to this idea.
    /// </summary>
    public int Contributors { get; set; }

    /// <summary>
    /// The number of amendments made to this idea.
    /// </summary>
    public int Amendments { get; set; }

    /// <summary>
    /// The total number of supporters across all amendments for this idea.
    /// </summary>
    public int Supporters { get; set; }

    /// <summary>
    /// The total number of opponents across all amendments for this idea.
    /// </summary>
    public int Opponents { get; set; }

    /// <summary>
    /// The ID of the source signal.
    /// </summary>
    public string SourceSignalId { get; set; } = string.Empty;

    /// <summary>
    /// The title of the source signal.
    /// </summary>
    public string SourceSignalTitle { get; set; } = string.Empty;
}
