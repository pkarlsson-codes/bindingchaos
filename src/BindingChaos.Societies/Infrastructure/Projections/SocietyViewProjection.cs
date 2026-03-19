using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Domain.Societies.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Societies.Infrastructure.Projections;

/// <summary>
/// Marten projection that builds <see cref="SocietyView"/> from Society events.
/// </summary>
internal sealed class SocietyViewProjection : SingleStreamProjection<SocietyView, string>
{
    /// <summary>
    /// Creates the initial <see cref="SocietyView"/> from a <see cref="SocietyCreated"/> event.
    /// </summary>
    /// <param name="e">The society created event.</param>
    /// <returns>The initialized society view.</returns>
    public static SocietyView Create(IEvent<SocietyCreated> e)
    {
        return new SocietyView
        {
            Id = e.Data.AggregateId,
            Name = e.Data.Name,
            Description = e.Data.Description,
            CreatedBy = e.Data.CreatedBy,
            CreatedAt = e.Data.OccurredAt,
            Tags = [.. e.Data.Tags],
            HasGeographicBounds = e.Data.GeographicBoundsJson is not null,
            GeographicBoundsJson = e.Data.GeographicBoundsJson,
            CenterJson = e.Data.CenterJson,
            Relationships = [],
            ActiveMemberCount = 0,
        };
    }

    /// <summary>
    /// Applies a <see cref="SocietyDescriptionUpdated"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyView view, SocietyDescriptionUpdated e)
    {
        view.Description = e.NewDescription;
    }

    /// <summary>
    /// Applies a <see cref="SocietyTagAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyView view, SocietyTagAdded e)
    {
        if (!view.Tags.Contains(e.Tag))
        {
            view.Tags.Add(e.Tag);
        }
    }

    /// <summary>
    /// Applies a <see cref="SocietyTagRemoved"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyView view, SocietyTagRemoved e)
    {
        view.Tags.Remove(e.Tag);
    }

    /// <summary>
    /// Applies a <see cref="SocietyRelationshipAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyView view, SocietyRelationshipAdded e)
    {
        view.Relationships.Add(new SocietyRelationshipView
        {
            TargetSocietyId = e.TargetSocietyId,
            RelationshipType = e.RelationshipType,
        });
    }

    /// <summary>
    /// Applies a <see cref="SocietyRelationshipRemoved"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyView view, SocietyRelationshipRemoved e)
    {
        view.Relationships.RemoveAll(r =>
            r.TargetSocietyId == e.TargetSocietyId && r.RelationshipType == e.RelationshipType);
    }

    /// <summary>
    /// Applies a <see cref="MemberJoined"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyView view, MemberJoined e)
    {
        view.ActiveMemberCount++;
    }

    /// <summary>
    /// Applies a <see cref="MemberLeft"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyView view, MemberLeft e)
    {
        if (view.ActiveMemberCount > 0)
        {
            view.ActiveMemberCount--;
        }
    }
}
