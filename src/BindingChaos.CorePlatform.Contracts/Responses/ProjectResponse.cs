namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for detailed project information.
/// </summary>
/// <param name="Id">The project identifier.</param>
/// <param name="UserGroupId">The owning user group identifier.</param>
/// <param name="Title">The project title.</param>
/// <param name="Description">The project description.</param>
/// <param name="CreatedAt">When the project was created.</param>
/// <param name="LastUpdatedAt">When the project was last updated.</param>
/// <param name="Amendments">The amendments proposed against this project.</param>
public sealed record ProjectResponse(
    string Id,
    string UserGroupId,
    string Title,
    string Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdatedAt,
    IReadOnlyCollection<ProjectResponse.Amendment> Amendments)
{
    /// <summary>
    /// Response model for a project amendment.
    /// </summary>
    /// <param name="Id">The amendment identifier.</param>
    /// <param name="ProposedById">The participant who proposed the amendment.</param>
    /// <param name="ProposedAt">When the amendment was proposed.</param>
    /// <param name="Status">The amendment status.</param>
    /// <param name="ContestedById">The participant contesting the amendment, when contested.</param>
    /// <param name="ContestedAt">When the amendment was contested, when contested.</param>
    /// <param name="LastStatusChangedAt">When the amendment status was last changed.</param>
    public sealed record Amendment(
        string Id,
        string ProposedById,
        DateTimeOffset ProposedAt,
        string Status,
        string? ContestedById,
        DateTimeOffset? ContestedAt,
        DateTimeOffset LastStatusChangedAt);
}
