using System.Linq.Expressions;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>User group list item read model.</summary>
public class UserGroupListItemView
{
    /// <summary>Maps sortable property names to expressions for ordering query results.</summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<UserGroupListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<UserGroupListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = x => x.Name,
            ["formedAt"] = x => x.FormedAt,
            ["memberCount"] = x => x.MemberCount,
        };

    /// <summary>The unique identifier of the user group.</summary>
    required public string Id { get; set; }

    /// <summary>The identifier of the commons this group governs.</summary>
    required public string CommonsId { get; set; }

    /// <summary>The name of the user group.</summary>
    required public string Name { get; set; }

    /// <summary>The philosophy of the user group.</summary>
    required public string Philosophy { get; set; }

    /// <summary>The identifier of the participant who founded the group.</summary>
    required public string FounderId { get; set; }

    /// <summary>The timestamp when the group was formed.</summary>
    required public DateTimeOffset FormedAt { get; set; }

    /// <summary>The current number of members in the group.</summary>
    required public int MemberCount { get; set; }

    /// <summary>The join policy name for the group.</summary>
    required public string JoinPolicy { get; set; }
}
