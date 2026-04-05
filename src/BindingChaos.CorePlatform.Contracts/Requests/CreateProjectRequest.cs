namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for creating a project in a user group.
/// </summary>
/// <param name="UserGroupId">The owning user group identifier.</param>
/// <param name="Title">The project title.</param>
/// <param name="Description">The project description.</param>
public sealed record CreateProjectRequest(
    string UserGroupId,
    string Title,
    string Description);
