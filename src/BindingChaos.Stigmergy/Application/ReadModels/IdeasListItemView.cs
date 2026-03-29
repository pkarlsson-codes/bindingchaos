using System.Linq.Expressions;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// A lightweight read model for idea list views.
/// </summary>
public class IdeasListItemView
{
    /// <summary>
    /// Sort mappings for idea list queries.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<IdeasListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<IdeasListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["createdAt"] = x => x.CreatedAt,
            ["lastUpdatedAt"] = x => x.LastUpdatedAt,
            ["title"] = x => x.Title,
        };

    /// <summary>Gets or sets the idea ID.</summary>
    required public string Id { get; set; }

    /// <summary>Gets or sets the ID of the participant who authored the idea.</summary>
    required public string AuthorId { get; set; }

    /// <summary>Gets or sets the idea title.</summary>
    required public string Title { get; set; }

    /// <summary>Gets or sets the idea description.</summary>
    required public string Description { get; set; }

    /// <summary>Gets or sets the idea status.</summary>
    required public string Status { get; set; }

    /// <summary>Gets or sets the ID of the parent idea this was forked from, if any.</summary>
    public string? ParentIdeaId { get; set; }

    /// <summary>Gets or sets when the idea was created.</summary>
    required public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets when the idea was last updated.</summary>
    required public DateTimeOffset LastUpdatedAt { get; set; }
}
