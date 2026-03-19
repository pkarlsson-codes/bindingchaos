using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Domain.Societies.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Societies.Infrastructure.Projections;

/// <summary>
/// Marten projection that builds <see cref="SocietyListItemView"/> from Society events.
/// </summary>
internal sealed class SocietyListItemViewProjection : SingleStreamProjection<SocietyListItemView, string>
{
    /// <summary>
    /// Creates the initial <see cref="SocietyListItemView"/> from a <see cref="SocietyCreated"/> event.
    /// </summary>
    /// <param name="e">The society created event.</param>
    /// <returns>The initialized society list item view.</returns>
    public static SocietyListItemView Create(IEvent<SocietyCreated> e)
    {
        return new SocietyListItemView
        {
            Id = e.Data.AggregateId,
            Name = e.Data.Name,
            Description = e.Data.Description,
            CreatedAt = e.Data.OccurredAt,
            Tags = [.. e.Data.Tags],
            HasGeographicBounds = e.Data.GeographicBoundsJson is not null,
            ActiveMemberCount = 0,
        };
    }

    /// <summary>
    /// Applies a <see cref="SocietyDescriptionUpdated"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyListItemView view, SocietyDescriptionUpdated e)
    {
        view.Description = e.NewDescription;
    }

    /// <summary>
    /// Applies a <see cref="SocietyTagAdded"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyListItemView view, SocietyTagAdded e)
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
    public static void Apply(SocietyListItemView view, SocietyTagRemoved e)
    {
        view.Tags.Remove(e.Tag);
    }

    /// <summary>
    /// Applies a <see cref="MemberJoined"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyListItemView view, MemberJoined e)
    {
        view.ActiveMemberCount++;
    }

    /// <summary>
    /// Applies a <see cref="MemberLeft"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(SocietyListItemView view, MemberLeft e)
    {
        if (view.ActiveMemberCount > 0)
        {
            view.ActiveMemberCount--;
        }
    }
}
