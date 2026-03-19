using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Instantiates a new instance of the <see cref="RequirementSpec"/> class.
/// </summary>
/// <param name="label">The label.</param>
/// <param name="quantity">The quantity.</param>
/// <param name="unit">The unit.</param>
/// <param name="type">The type.</param>
public class RequirementSpec(string label, double quantity, string unit, RequirementType type) : ValueObject
{
    /// <summary>
    /// Human readable label for the requirement.
    /// </summary>
    public string Label { get; } = label;

    /// <summary>
    /// Required quantity.
    /// </summary>
    public double Quantity { get; } = quantity;

    /// <summary>
    /// Unit of quantity.
    /// </summary>
    public string Unit { get; } = unit;

    /// <summary>
    /// Type of requirement.
    /// </summary>
    public RequirementType Type { get; set; } = type;

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Label;
        yield return Quantity;
        yield return Unit;
        yield return Type;
    }
}
