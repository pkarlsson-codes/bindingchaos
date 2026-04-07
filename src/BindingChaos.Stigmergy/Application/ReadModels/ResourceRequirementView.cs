namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model representing the current state of a resource requirement, including all pledges.
/// </summary>
public sealed class ResourceRequirementView
{
    /// <summary>Gets or sets the id of the resource requirement.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the id of the owning project.</summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>Gets or sets the human-readable description of what is needed.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the total quantity of the resource required.</summary>
    public double QuantityNeeded { get; set; }

    /// <summary>Gets or sets the unit of measure (e.g. "hours", "kg").</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Gets or sets the timestamp when this requirement was created.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the timestamp when this requirement was last updated.</summary>
    public DateTimeOffset LastUpdatedAt { get; set; }

    /// <summary>Gets or sets the pledges made toward this requirement.</summary>
    public List<PledgeView> Pledges { get; set; } = [];

    /// <summary>
    /// Read model representing a single pledge against a resource requirement.
    /// </summary>
    public sealed class PledgeView
    {
        /// <summary>Gets or sets the id of the pledge.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the id of the participant who made the pledge.</summary>
        public string PledgedById { get; set; } = string.Empty;

        /// <summary>Gets or sets the timestamp when the pledge was made.</summary>
        public DateTimeOffset PledgedAt { get; set; }

        /// <summary>Gets or sets the amount of resources pledged.</summary>
        public double Amount { get; set; }

        /// <summary>Gets or sets a value indicating whether this pledge has been withdrawn.</summary>
        public bool IsWithdrawn { get; set; }
    }
}
