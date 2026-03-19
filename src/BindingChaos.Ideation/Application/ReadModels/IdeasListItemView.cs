using System.Linq.Expressions;

namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// Projection for listing ideas in the ideation system.
/// </summary>
public class IdeasListItemView
{
    /// <summary>
    /// Logical sort mappings for idea list queries.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<IdeasListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<IdeasListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["createdAt"] = x => x.CreatedAt,
            ["lastUpdatedAt"] = x => x.LastUpdatedAt,
            ["openAmendmentCount"] = x => x.OpenAmendmentCount,
            ["title"] = x => x.Title,
        };

    /// <summary>
    /// The unique identifier for the idea.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The title of the idea.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The body of the idea, which contains the main content or proposal.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// The number open amendments to this idea.
    /// </summary>
    public int OpenAmendmentCount { get; set; }

    /// <summary>
    /// The society context (governance jurisdiction) for this idea.
    /// </summary>
    public string SocietyContext { get; set; } = string.Empty;

    /// <summary>
    /// When the idea was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the idea was last updated.
    /// </summary>
    public DateTimeOffset LastUpdatedAt { get; set; }

    /// <summary>
    /// The tags associated with this idea, used for categorization or search.
    /// </summary>
    public List<string> SourceSignalIds { get; set; } = [];

    /// <summary>
    /// The tags associated with this idea.
    /// </summary>
    public IReadOnlyCollection<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets the identifier of the parent idea, if one exists.
    /// </summary>
    public string? ParentIdeaId { get; set; }
}