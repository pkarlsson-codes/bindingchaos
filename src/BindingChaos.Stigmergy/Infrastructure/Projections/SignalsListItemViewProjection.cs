using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Signals.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects signal events into <see cref="SignalsListItemView"/>.
/// </summary>
internal sealed class SignalsListItemViewProjection : SingleStreamProjection<SignalsListItemView, string>
{
    /// <summary>
    /// Creates a new <see cref="SignalsListItemView"/> from a <see cref="SignalCaptured"/> event.
    /// </summary>
    /// <param name="e">The captured event.</param>
    /// <returns>A new <see cref="SignalsListItemView"/>.</returns>
    public static SignalsListItemView Create(IEvent<SignalCaptured> e) =>
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
    /// Updates amplification count and tracks the amplifier on <see cref="SignalAmplified"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The amplified event.</param>
    public static void Apply(SignalsListItemView view, SignalAmplified e)
    {
        view.AmplifierIds.Add(e.AmplifiedById);
        view.AmplificationCount = view.AmplifierIds.Count;
        view.LastAmplifiedAt = e.OccurredAt;
    }

    /// <summary>
    /// Decrements amplification count and removes the amplifier on <see cref="SignalAmplificationWithdrawn"/>.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The withdrawal event.</param>
    public static void Apply(SignalsListItemView view, SignalAmplificationWithdrawn e)
    {
        view.AmplifierIds.Remove(e.AmplifierId);
        view.AmplificationCount = view.AmplifierIds.Count;
    }
}
