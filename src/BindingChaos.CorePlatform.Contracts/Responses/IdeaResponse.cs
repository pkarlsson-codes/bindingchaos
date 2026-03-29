namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for idea information.
/// </summary>
/// <param name="Id">The ID of the idea.</param>
/// <param name="Title">The title of the idea.</param>
/// <param name="Description">The description of the idea.</param>
/// <param name="AuthorPseudonym">The pseudonym of the author who created the idea.</param>
/// <param name="CreatedAt">When the idea was created.</param>
/// <param name="LastUpdatedAt">When the idea was last updated.</param>
/// <param name="Status">The status of the idea.</param>
public sealed record IdeaResponse(
    string Id,
    string Title,
    string Description,
    string AuthorPseudonym,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdatedAt,
    string Status);
