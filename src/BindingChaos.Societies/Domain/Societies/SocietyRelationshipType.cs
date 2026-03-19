using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Societies.Domain.Societies;

/// <summary>
/// Defines the type of relationship between two societies.
/// </summary>
public sealed class SocietyRelationshipType : Enumeration<SocietyRelationshipType>
{
    /// <summary>
    /// The society is a part of another society.
    /// </summary>
    public static readonly SocietyRelationshipType PartOf = new(0, nameof(PartOf));

    /// <summary>
    /// The society is affiliated with another society.
    /// </summary>
    public static readonly SocietyRelationshipType Affiliated = new(1, nameof(Affiliated));

    /// <summary>
    /// The society is federated with another society.
    /// </summary>
    public static readonly SocietyRelationshipType Federated = new(2, nameof(Federated));

    private SocietyRelationshipType(int value, string displayName)
        : base(value, displayName)
    {
    }
}
