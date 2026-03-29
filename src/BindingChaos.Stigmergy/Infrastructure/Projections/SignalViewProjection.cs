using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Signals.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects signal events into <see cref="SignalView"/>.
/// </summary>
internal sealed class SignalViewProjection : SingleStreamProjection<SignalView, string>
{
    /// <summary>
    /// Creates a new <see cref="SignalView"/> from a <see cref="SignalCaptured"/> event.
    /// </summary>
    /// <param name="e">The captured event.</param>
    /// <returns>A new <see cref="SignalView"/>.</returns>
    public static SignalView Create(IEvent<SignalCaptured> e) =>
        new()
        {
            Id = e.Data.AggregateId,
            Title = e.Data.Title,
            Description = e.Data.Description,
            CapturedById = e.Data.CapturedById,
            CapturedAt = e.Data.OccurredAt,
            Tags = e.Data.Tags == null ? [] : [.. e.Data.Tags],
            AttachmentIds = e.Data.AttachmentIds == null ? [] : [.. e.Data.AttachmentIds],
            AmplificationCount = 0,
            Latitude = e.Data.Latitude,
            Longitude = e.Data.Longitude,
        };

    /// <summary>
    /// Adds an amplification entry and updates the count on <see cref="SignalAmplified"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The amplified event.</param>
    public static void Apply(SignalView view, IEvent<SignalAmplified> e)
    {
        view.Amplifications.Add(new SignalView.AmplificationEntry
        {
            AmplifiedById = e.Data.AmplifiedById,
            AmplifiedAt = e.Data.OccurredAt,
        });
        view.AmplificationCount = view.Amplifications.Count;
        view.LastAmplifiedAt = e.Data.OccurredAt;
    }

    /// <summary>
    /// Removes the amplification entry and updates the count on <see cref="SignalAmplificationWithdrawn"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The withdrawal event.</param>
    public static void Apply(SignalView view, SignalAmplificationWithdrawn e)
    {
        view.Amplifications.RemoveAll(a => a.AmplifiedById == e.AmplifierId);
        view.AmplificationCount = view.Amplifications.Count;
    }
}
