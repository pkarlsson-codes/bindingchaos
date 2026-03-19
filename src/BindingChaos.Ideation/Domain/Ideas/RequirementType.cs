using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Specifies the types of requirements that can be defined within a system.
/// </summary>
public class RequirementType : Enumeration<RequirementType>
{
    /// <summary>
    /// Represents an asset type in which each unit is interchangeable with any other unit of the same type.
    /// </summary>
    public static readonly RequirementType Fungible = new(1, nameof(Fungible));

    /// <summary>
    /// Represents a discrete value in a mathematical or computational context.
    /// </summary>
    public static readonly RequirementType Discrete = new(2, nameof(Discrete));

    /// <summary>
    /// Represents a set of skills or abilities that an individual possesses in a specific domain.
    /// </summary>
    public static readonly RequirementType Competence = new(3, nameof(Competence));

    /// <summary>
    /// Provides informational content related to the system or application state.
    /// </summary>
    public static readonly RequirementType Informational = new(4, nameof(Informational));

    private RequirementType(int value, string displayName)
        : base(value, displayName)
    {
    }
}
