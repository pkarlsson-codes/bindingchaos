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
}
