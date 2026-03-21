using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// A resource requirement on a project, which participants may pledge to fulfil.
/// </summary>
public sealed class Requirement : Entity<RequirementId>
{
    private readonly List<Pledge> _pledges = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Requirement"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for this requirement.</param>
    /// <param name="description">A human-readable description of what is needed.</param>
    /// <param name="quantityNeeded">The total quantity of the resource required.</param>
    /// <param name="unit">The unit of measure for the quantity (e.g. "hours", "kg").</param>
    internal Requirement(RequirementId id, string description, double quantityNeeded, string unit)
    {
        Id = id;
        Description = description;
        QuantityNeeded = quantityNeeded;
        Unit = unit;
    }

    /// <summary>
    /// Gets the human-readable description of what is needed.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the total quantity of the resource required.
    /// </summary>
    public double QuantityNeeded { get; }

    /// <summary>
    /// Gets the unit of measure for the quantity (e.g. "hours", "kg").
    /// </summary>
    public string Unit { get; }

    /// <summary>
    /// Gets the pledges made toward this requirement.
    /// </summary>
    public IReadOnlyList<Pledge> Pledges => _pledges.AsReadOnly();
}
