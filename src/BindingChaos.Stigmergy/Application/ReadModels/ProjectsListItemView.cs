using System.Linq.Expressions;
using BindingChaos.Stigmergy.Application.DTOs;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// A lightweight read model for listing projects.
/// </summary>
public class ProjectsListItemView
{
    /// <summary>
    /// Sort mappings for project list queries.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<ProjectsListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<ProjectsListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["createdAt"] = x => x.CreatedAt,
            ["lastUpdatedAt"] = x => x.LastUpdatedAt,
            ["title"] = x => x.Title,
            ["activeAmendmentCount"] = x => x.ActiveAmendmentCount,
            ["contestedAmendmentCount"] = x => x.ContestedAmendmentCount,
            ["rejectedAmendmentCount"] = x => x.RejectedAmendmentCount,
        };

    /// <summary>Gets or sets the project ID.</summary>
    required public string Id { get; set; }

    /// <summary>Gets or sets the owning user group ID.</summary>
    required public string UserGroupId { get; set; }

    /// <summary>Gets or sets the project title.</summary>
    required public string Title { get; set; }

    /// <summary>Gets or sets the project description.</summary>
    required public string Description { get; set; }

    /// <summary>Gets or sets when the project was created.</summary>
    required public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets when the project was last updated.</summary>
    required public DateTimeOffset LastUpdatedAt { get; set; }

    /// <summary>Gets or sets the number of active amendments.</summary>
    required public int ActiveAmendmentCount { get; set; }

    /// <summary>Gets or sets the number of contested amendments.</summary>
    required public int ContestedAmendmentCount { get; set; }

    /// <summary>Gets or sets the number of rejected amendments.</summary>
    required public int RejectedAmendmentCount { get; set; }

    /// <summary>Gets or sets the lifecycle status of the project.</summary>
    required public ProjectStatusDto Status { get; set; }
}
