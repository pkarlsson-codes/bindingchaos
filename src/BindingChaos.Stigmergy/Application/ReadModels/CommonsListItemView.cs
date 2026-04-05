using System.Linq.Expressions;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>Commons list item read model.</summary>
public class CommonsListItemView
{
    /// <summary>Maps sortable property names to expressions for ordering query results.</summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<CommonsListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<CommonsListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = x => x.Name,
            ["proposedAt"] = x => x.ProposedAt,
        };

    /// <summary>The unique identifier of the commons.</summary>
    required public string Id { get; set; }

    /// <summary>The name of the commons.</summary>
    required public string Name { get; set; }

    /// <summary>The description of the commons.</summary>
    required public string Description { get; set; }

    /// <summary>The lifecycle status of the commons.</summary>
    required public string Status { get; set; }

    /// <summary>The identifier of the participant who proposed the commons.</summary>
    required public string FounderId { get; set; }

    /// <summary>The timestamp when the commons was proposed.</summary>
    required public DateTimeOffset ProposedAt { get; set; }

    /// <summary>The identifiers of concerns linked to this commons.</summary>
    public List<string> LinkedConcernIds { get; set; } = [];
}
