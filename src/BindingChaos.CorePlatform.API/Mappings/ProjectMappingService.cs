using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Stigmergy.Application.ReadModels;

namespace BindingChaos.CorePlatform.API.Mappings;

/// <summary>
/// Pure structural mapping from project read models to API response contracts.
/// </summary>
internal static class ProjectMapper
{
    /// <summary>
    /// Maps a <see cref="ProjectView"/> into a <see cref="ProjectResponse"/>.
    /// </summary>
    /// <param name="project">The project read model to map.</param>
    /// <returns>The mapped response.</returns>
    internal static ProjectResponse ToProjectResponse(ProjectView project)
    {
        var amendments = project.Amendments
            .Select(a => new ProjectResponse.Amendment(
                a.Id,
                a.ProposedById,
                a.ProposedAt,
                a.Status,
                a.ContestedById,
                a.ContestedAt,
                a.LastStatusChangedAt))
            .ToArray();

        return new ProjectResponse(
            project.Id,
            project.UserGroupId,
            project.Title,
            project.Description,
            project.CreatedAt,
            project.LastUpdatedAt,
            amendments);
    }

    /// <summary>
    /// Maps a <see cref="ProjectsListItemView"/> into a <see cref="ProjectListItemResponse"/>.
    /// </summary>
    /// <param name="project">The list-item read model to map.</param>
    /// <returns>The mapped response.</returns>
    internal static ProjectListItemResponse ToProjectListItemResponse(ProjectsListItemView project)
    {
        return new ProjectListItemResponse(
            project.Id,
            project.UserGroupId,
            project.Title,
            project.Description,
            project.CreatedAt,
            project.LastUpdatedAt,
            project.ActiveAmendmentCount,
            project.ContestedAmendmentCount,
            project.RejectedAmendmentCount);
    }
}
