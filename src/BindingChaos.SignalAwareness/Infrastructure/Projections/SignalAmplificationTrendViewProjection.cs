using BindingChaos.SignalAwareness.Application.ReadModels;
using BindingChaos.SignalAwareness.Domain.Signals.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.SignalAwareness.Infrastructure.Projections;

/// <summary>
/// Projection for signal amplification trend information over time.
/// Tracks individual amplification events over time for frontend aggregation.
/// </summary>
internal sealed class SignalAmplificationTrendViewProjection : SingleStreamProjection<SignalAmplificationTrendView, string>
{
    /// <summary>
    /// Creates a new instance of <see cref="SignalAmplificationTrendView"/> from the <see cref="SignalCaptured"/> event.
    /// </summary>
    /// <param name="e">The event containing the signal capture details.</param>
    /// <returns>A new instance of <see cref="SignalAmplificationTrendView"/>.</returns>
    public static SignalAmplificationTrendView Create(SignalCaptured e)
    {
        return new SignalAmplificationTrendView
        {
            Id = e.AggregateId,
            SignalId = e.AggregateId,
            DataPoints = [], // Start with empty list - no amplifications yet
        };
    }

    /// <summary>
    /// Applies the <see cref="SignalAmplified"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(SignalAmplificationTrendView view, SignalAmplified e)
    {
        view.DataPoints.Add(new SignalAmplificationTrendPoint
        {
            Date = e.OccurredAt,
            EventType = "amplify",
        });
    }

    /// <summary>
    /// Applies the <see cref="SignalAmplificationAttenuated"/> event to the view.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The event to apply.</param>
    public static void Apply(SignalAmplificationTrendView view, SignalAmplificationAttenuated e)
    {
        view.DataPoints.Add(new SignalAmplificationTrendPoint
        {
            Date = e.OccurredAt,
            EventType = "attenuate",
        });
    }
}
