using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.ResourceRequirements.Events;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements;

/// <summary>
/// Aggregate root representing a resource requirement on a project,
/// which participants may pledge resources toward.
/// </summary>
public sealed class ResourceRequirement : AggregateRoot<ResourceRequirementId>
{
    private readonly List<Pledge> _pledges = [];

#pragma warning disable CS8618
    private ResourceRequirement() { }
#pragma warning restore CS8618

    /// <summary>Gets the id of the project this requirement belongs to.</summary>
    public ProjectId ProjectId { get; private set; }

    /// <summary>Gets a human-readable description of what is needed.</summary>
    public string Description { get; private set; }

    /// <summary>Gets the total quantity of the resource required.</summary>
    public double QuantityNeeded { get; private set; }

    /// <summary>Gets the unit of measure for the quantity (e.g. "hours", "kg").</summary>
    public string Unit { get; private set; }

    /// <summary>Gets the pledges made toward this requirement.</summary>
    public IReadOnlyList<Pledge> Pledges => _pledges.AsReadOnly();

    /// <summary>
    /// Creates a new resource requirement for the specified project.
    /// </summary>
    /// <param name="projectId">The id of the owning project.</param>
    /// <param name="description">A human-readable description of what is needed.</param>
    /// <param name="quantityNeeded">The total quantity of the resource required.</param>
    /// <param name="unit">The unit of measure (e.g. "hours", "kg").</param>
    /// <returns>A new <see cref="ResourceRequirement"/> instance.</returns>
    public static ResourceRequirement Create(
        ProjectId projectId,
        string description,
        double quantityNeeded,
        string unit)
    {
        ArgumentNullException.ThrowIfNull(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);

        var requirement = new ResourceRequirement();
        var id = ResourceRequirementId.Generate();
        requirement.ApplyChange(new RequirementAdded(id.Value, projectId.Value, description, quantityNeeded, unit));
        return requirement;
    }

    /// <summary>
    /// Updates the quantity of resource needed.
    /// </summary>
    /// <param name="quantityNeeded">The new quantity needed.</param>
    public void UpdateQuantity(double quantityNeeded)
        => ApplyChange(new RequirementQuantityUpdated(Id.Value, quantityNeeded));

    /// <summary>
    /// Pledges resources toward this requirement.
    /// </summary>
    /// <param name="pledgedById">The participant making the pledge.</param>
    /// <param name="amount">The amount of resources being pledged.</param>
    /// <returns>The id of the newly created pledge.</returns>
    public PledgeId PledgeResources(ParticipantId pledgedById, double amount)
    {
        ArgumentNullException.ThrowIfNull(pledgedById);

        var pledgeId = PledgeId.Generate();
        ApplyChange(new ResourcesPledged(Id.Value, pledgeId.Value, pledgedById.Value, amount));
        return pledgeId;
    }

    /// <summary>
    /// Withdraws a previously made pledge.
    /// </summary>
    /// <param name="pledgeId">The id of the pledge to withdraw.</param>
    /// <param name="actorId">The participant attempting to withdraw.</param>
    /// <exception cref="BusinessRuleViolationException">
    /// Thrown if the pledge cannot be found, is already withdrawn, or the actor is not the original pledger.
    /// </exception>
    public void WithdrawPledge(PledgeId pledgeId, ParticipantId actorId)
    {
        ArgumentNullException.ThrowIfNull(pledgeId);
        ArgumentNullException.ThrowIfNull(actorId);

        var pledge = _pledges.FirstOrDefault(p => p.Id == pledgeId)
            ?? throw new BusinessRuleViolationException("Unable to find pledge to withdraw.");

        if (pledge.IsWithdrawn)
        {
            throw new BusinessRuleViolationException("This pledge has already been withdrawn.");
        }

        if (pledge.PledgedById != actorId)
        {
            throw new BusinessRuleViolationException("Only the participant who made the pledge can withdraw it.");
        }

        ApplyChange(new PledgeWithdrawn(Id.Value, pledgeId.Value));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case RequirementAdded e: Apply(e); break;
            case RequirementQuantityUpdated e: Apply(e); break;
            case ResourcesPledged e: Apply(e); break;
            case PledgeWithdrawn e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(RequirementAdded e)
    {
        Id = ResourceRequirementId.Create(e.AggregateId);
        ProjectId = ProjectId.Create(e.ProjectId);
        Description = e.Description;
        QuantityNeeded = e.QuantityNeeded;
        Unit = e.Unit;
    }

    private void Apply(RequirementQuantityUpdated e) => QuantityNeeded = e.QuantityNeeded;

    private void Apply(ResourcesPledged e)
        => _pledges.Add(new Pledge(
            PledgeId.Create(e.PledgeId),
            ParticipantId.Create(e.PledgedById),
            e.OccurredAt,
            e.Amount));

    private void Apply(PledgeWithdrawn e)
        => _pledges.Single(p => p.Id == PledgeId.Create(e.PledgeId)).IsWithdrawn = true;
}
