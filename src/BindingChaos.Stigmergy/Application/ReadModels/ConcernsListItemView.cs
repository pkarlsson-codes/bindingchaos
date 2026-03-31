using System.Linq.Expressions;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Concerns list item read model.
/// </summary>
public class ConcernsListItemView
{
    /// <summary>
    /// Maps sortable property names to expressions for ordering query results.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<ConcernsListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<ConcernsListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = x => x.Name,
        };

    /// <summary>
    /// The unique identifier of the concern.
    /// </summary>
    required public string Id { get; set; }

    /// <summary>
    /// The identifier of the actor who raised the concern.
    /// </summary>
    required public string RaisedById { get; set; }

    /// <summary>
    /// The name of the concern.
    /// </summary>
    required public string Name { get; set; }

    /// <summary>
    /// The tags associated with the concern.
    /// </summary>
    required public List<string> Tags { get; set; }

    /// <summary>
    /// The IDs of the signals related to this concern.
    /// </summary>
    required public List<string> SignalIds { get; set; }
}
