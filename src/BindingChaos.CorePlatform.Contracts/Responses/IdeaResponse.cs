namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for idea information.
/// </summary>
/// <param name="Id">The ID of the idea.</param>
/// <param name="Title">The title of the idea.</param>
/// <param name="Body">The description of the idea.</param>
/// <param name="SocietyContext">The ID of the society where this idea is proposed.</param>
/// <param name="SourceSignals">The source signals associated with this idea.</param>
/// <param name="AuthorPseudonym">The pseudonym of the author who created the idea.</param>
/// <param name="OpenAmendmentCount">The number of amendments made to this idea.</param>
/// <param name="CreatedAt">When the idea was created.</param>
/// <param name="LastUpdatedAt">When the idea was last updated.</param>
/// <param name="Tags">The tags associated with this idea.</param>
/// <param name="Status">The status of the idea.</param>
public sealed record IdeaResponse(
    string Id,
    string Title,
    string Body,
    string SocietyContext,
    IdeaSourceSignal[] SourceSignals,
    string AuthorPseudonym,
    int OpenAmendmentCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdatedAt,
    string[] Tags,
    string Status);

/// <summary>
/// Represents a source signal associated with an idea.
/// </summary>
/// <param name="Id">The unique identifier of the source signal.</param>
/// <param name="Title">The title of the source signal.</param>
public sealed record IdeaSourceSignal(string Id, string Title);
