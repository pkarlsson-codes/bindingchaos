using System.Linq.Expressions;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>Concerns list item read model.</summary>
public class ConcernsListItemView
{
    /// <summary>Maps sortable property names to expressions for ordering query results.</summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<ConcernsListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<ConcernsListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = x => x.Name,
        };

    /// <summary>The unique identifier of the concern.</summary>
    required public string Id { get; set; }

    /// <summary>The identifiers of the participants affected by the concern.</summary>
    public List<string> AffectedParticipantIds { get; set; } = [];

    /// <summary>The count of participants affected by the concern.</summary>
    public int AffectedCount => AffectedParticipantIds.Count;

    /// <summary>The identifier of the actor who raised the concern.</summary>
    required public string RaisedById { get; set; }

    /// <summary>The name of the concern.</summary>
    required public string Name { get; set; }

    /// <summary>The tags associated with the concern.</summary>
    required public List<string> Tags { get; set; }

    /// <summary>The signals related to this concern.</summary>
    public List<ReferenceSignal> Signals { get; set; } = [];

    /// <summary>Represents a signal referenced by a concern.</summary>
    public class ReferenceSignal
    {
        /// <summary>The unique identifier of the signal.</summary>
        required public string Id { get; set; }

        /// <summary>The title of the signal.</summary>
        required public string Title { get; set; }
    }
}
