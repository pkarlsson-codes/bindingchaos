using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.ResourceRequirements.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projection for the <see cref="ResourceRequirementView"/> read model.
/// One document per resource requirement stream.
/// </summary>
internal sealed class ResourceRequirementViewProjection : SingleStreamProjection<ResourceRequirementView, string>
{
    /// <summary>
    /// Creates the initial view when a resource requirement is added.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <returns>A new <see cref="ResourceRequirementView"/> instance.</returns>
    public static ResourceRequirementView Create(RequirementAdded e) => new()
    {
        Id = e.AggregateId,
        ProjectId = e.ProjectId,
        Description = e.Description,
        QuantityNeeded = e.QuantityNeeded,
        Unit = e.Unit,
        CreatedAt = e.OccurredAt,
        LastUpdatedAt = e.OccurredAt,
    };

    /// <summary>
    /// Updates the quantity when it changes.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="view">The current view.</param>
    public static void Apply(RequirementQuantityUpdated e, ResourceRequirementView view)
    {
        view.QuantityNeeded = e.QuantityNeeded;
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Adds a pledge to the view.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="view">The current view.</param>
    public static void Apply(ResourcesPledged e, ResourceRequirementView view)
    {
        view.Pledges.Add(new ResourceRequirementView.PledgeView
        {
            Id = e.PledgeId,
            PledgedById = e.PledgedById,
            PledgedAt = e.OccurredAt,
            Amount = e.Amount,
            IsWithdrawn = false,
        });
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Marks a pledge as withdrawn.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="view">The current view.</param>
    public static void Apply(PledgeWithdrawn e, ResourceRequirementView view)
    {
        var pledge = view.Pledges.Single(p => p.Id == e.PledgeId);
        pledge.IsWithdrawn = true;
        view.LastUpdatedAt = e.OccurredAt;
    }
}
