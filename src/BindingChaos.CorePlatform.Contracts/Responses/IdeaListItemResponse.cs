namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for a list item of an idea.
/// </summary>
/// <param name="Id">The unique identifier of the idea.</param>
/// <param name="Title">The title of the idea.</param>
/// <param name="Description">The body content describing the idea.</param>
/// <param name="CreatedAt">The timestamp when the idea was originally created.</param>
/// <param name="LastUpdatedAt">The timestamp when the idea was last modified or updated.</param>
/// <param name="Status">The current status of the idea (e.g., "Draft", "Published").</param>
public record IdeaListItemResponse(
    string Id,
    string Title,
    string Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdatedAt,
    string Status);
