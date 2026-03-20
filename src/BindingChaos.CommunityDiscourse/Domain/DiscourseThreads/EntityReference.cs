using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;

/// <summary>
/// Value object representing a reference to an entity that discourse is about.
/// </summary>
public sealed class EntityReference : ValueObject
{
    private static readonly HashSet<string> ValidEntityTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "idea", "signal", "amendment",
    };

    /// <summary>
    /// Initializes a new instance of the EntityReference class.
    /// </summary>
    /// <param name="entityType">The type of entity being referenced.</param>
    /// <param name="entityId">The identifier of the entity being referenced.</param>
    private EntityReference(string entityType, string entityId)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Gets the type of entity being referenced (e.g., "idea", "signal", "amendment").
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the identifier of the entity being referenced.
    /// </summary>
    public string EntityId { get; }

    /// <summary>
    /// Creates a new EntityReference.
    /// </summary>
    /// <param name="entityType">The type of entity being referenced.</param>
    /// <param name="entityId">The identifier of the entity being referenced.</param>
    /// <returns>A new EntityReference instance.</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
    public static EntityReference Create(string entityType, string entityId)
    {
        ValidateEntityType(entityType);
        ValidateEntityId(entityId);
        return new EntityReference(entityType.ToLowerInvariant(), entityId);
    }

    /// <summary>
    /// Tries to create a new EntityReference.
    /// </summary>
    /// <param name="entityType">The type of entity being referenced.</param>
    /// <param name="entityId">The identifier of the entity being referenced.</param>
    /// <returns>A new EntityReference instance, or null if invalid.</returns>
    public static EntityReference? TryCreate(string? entityType, string? entityId)
    {
        if (string.IsNullOrWhiteSpace(entityType) || string.IsNullOrWhiteSpace(entityId))
        {
            return null;
        }

        try
        {
            ValidateEntityType(entityType);
            ValidateEntityId(entityId);
            return new EntityReference(entityType.ToLowerInvariant(), entityId);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns a string representation of this EntityReference.
    /// </summary>
    /// <returns>String representation in the format "entityType:entityId".</returns>
    public override string ToString()
    {
        return $"{EntityType}:{EntityId}";
    }

    /// <summary>
    /// Gets the components used for equality comparison.
    /// </summary>
    /// <returns>The components for equality comparison.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EntityType;
        yield return EntityId;
    }

    /// <summary>
    /// Validates the entity type.
    /// </summary>
    /// <param name="entityType">The entity type to validate.</param>
    /// <exception cref="ArgumentException">Thrown when entity type is invalid.</exception>
    private static void ValidateEntityType(string entityType)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException("Entity type cannot be null, empty, or whitespace.", nameof(entityType));
        }

        if (!ValidEntityTypes.Contains(entityType))
        {
            throw new ArgumentException($"Entity type must be one of: {string.Join(", ", ValidEntityTypes)}.", nameof(entityType));
        }
    }

    /// <summary>
    /// Validates the entity ID.
    /// </summary>
    /// <param name="entityId">The entity ID to validate.</param>
    /// <exception cref="ArgumentException">Thrown when entity ID is invalid.</exception>
    private static void ValidateEntityId(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
        {
            throw new ArgumentException("Entity ID cannot be null, empty, or whitespace.", nameof(entityId));
        }

        if (entityId.Length > 128)
        {
            throw new ArgumentException("Entity ID cannot exceed 128 characters.", nameof(entityId));
        }
    }
}