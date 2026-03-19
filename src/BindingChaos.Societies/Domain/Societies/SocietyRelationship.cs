using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Societies.Domain.Societies;

/// <summary>
/// Value object representing a directed relationship from one society to another.
/// </summary>
public sealed class SocietyRelationship : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SocietyRelationship"/> class.
    /// </summary>
    /// <param name="targetSocietyId">The target society of the relationship.</param>
    /// <param name="relationshipType">The type of relationship.</param>
    public SocietyRelationship(SocietyId targetSocietyId, SocietyRelationshipType relationshipType)
    {
        ArgumentNullException.ThrowIfNull(targetSocietyId);
        TargetSocietyId = targetSocietyId;
        RelationshipType = relationshipType;
    }

    /// <summary>
    /// Gets the target society of this relationship.
    /// </summary>
    public SocietyId TargetSocietyId { get; }

    /// <summary>
    /// Gets the type of this relationship.
    /// </summary>
    public SocietyRelationshipType RelationshipType { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TargetSocietyId.Value;
        yield return RelationshipType;
    }
}
