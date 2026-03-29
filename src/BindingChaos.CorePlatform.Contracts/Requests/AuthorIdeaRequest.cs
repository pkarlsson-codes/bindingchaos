namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for creating an idea.
/// </summary>
/// <param name="Title">The title of the idea.</param>
/// <param name="Description">The description of the idea.</param>
public record AuthorIdeaRequest(
    string Title,
    string Description);
