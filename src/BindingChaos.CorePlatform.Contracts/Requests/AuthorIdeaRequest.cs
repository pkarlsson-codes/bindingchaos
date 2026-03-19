namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for creating an idea.
/// </summary>
/// <param name="Title">The title of the idea.</param>
/// <param name="Body">The description of the idea.</param>
/// <param name="SocietyId">The ID of the society where the idea is proposed.</param>
/// <param name="SourceSignalIds">The source signal IDs that this idea is based on.</param>
/// <param name="Tags">The tags for the idea.</param>
public record AuthorIdeaRequest(
    string Title,
    string Body,
    string SocietyId,
    string[] SourceSignalIds,
    string[] Tags);
