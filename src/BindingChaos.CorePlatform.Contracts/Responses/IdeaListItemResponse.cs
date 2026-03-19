namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for a list item of an idea.
/// </summary>
/// <param name="Id">The unique identifier of the idea.</param>
/// <param name="Title">The title of the idea.</param>
/// <param name="Body">The body content describing the idea.</param>
/// <param name="SocietyContext">The ID of the society where this idea is proposed.</param>
/// <param name="SourceSignalIds">The collection of signal IDs that this idea was derived from or references.</param>
/// <param name="OpenAmendmentCount">The number of amendments that are currently open for voting on this idea.</param>
/// <param name="CreatedAt">The timestamp when the idea was originally created.</param>
/// <param name="LastUpdatedAt">The timestamp when the idea was last modified or updated.</param>
/// <param name="Tags">The collection of tags associated with this idea for categorization and discovery.</param>
/// <param name="Status">The current status of the idea (e.g., "published", "archived").</param>
public record IdeaListItemResponse(
    string Id,
    string Title,
    string Body,
    string SocietyContext,
    IReadOnlyCollection<string> SourceSignalIds,
    int OpenAmendmentCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdatedAt,
    IReadOnlyCollection<string> Tags,
    string Status);
