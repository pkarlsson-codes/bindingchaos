using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Domain.Societies.Events;
using Marten.Events.Projections;

namespace BindingChaos.Societies.Infrastructure.Projections;

/// <summary>
/// Projection that creates a <see cref="SocietyAffectedByCommonsView"/> per commons-society link.
/// </summary>
internal sealed class SocietyAffectedByCommonsViewProjection : EventProjection
{
    /// <summary>
    /// Creates a <see cref="SocietyAffectedByCommonsView"/> when a commons is linked to a society.
    /// </summary>
    /// <param name="e">The event containing the link data.</param>
    /// <returns>The new view document.</returns>
    public static SocietyAffectedByCommonsView Create(CommonsLinkedToSociety e)
    {
        return new SocietyAffectedByCommonsView
        {
            Id = $"{e.CommonsId}:{e.AggregateId}",
            CommonsId = e.CommonsId,
            SocietyId = e.AggregateId,
            DeclaredAt = e.OccurredAt,
        };
    }
}
