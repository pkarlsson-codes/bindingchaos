namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for project list items.
/// </summary>
/// <param name="Id">The project identifier.</param>
/// <param name="UserGroupId">The owning user group identifier.</param>
/// <param name="Title">The project title.</param>
/// <param name="Description">The project description.</param>
/// <param name="CreatedAt">When the project was created.</param>
/// <param name="LastUpdatedAt">When the project was last updated.</param>
/// <param name="ActiveAmendmentCount">The number of active amendments.</param>
/// <param name="ContestedAmendmentCount">The number of contested amendments.</param>
/// <param name="RejectedAmendmentCount">The number of rejected amendments.</param>
public sealed record ProjectListItemResponse(
    string Id,
    string UserGroupId,
    string Title,
    string Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdatedAt,
    int ActiveAmendmentCount,
    int ContestedAmendmentCount,
    int RejectedAmendmentCount);
