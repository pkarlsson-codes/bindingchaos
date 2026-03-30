using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Signals.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects amplification data points into the read model view.
/// </summary>
internal sealed class AmplificationTrendViewProjection
: SingleStreamProjection<AmplificationTrendView, string>
{
    /// <summary>
    /// Creates a new amplification data point view from a signal amplified event.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <returns>The <see cref="AmplificationTrendView"/>.</returns>
    public static AmplificationTrendView Create(IEvent<SignalAmplified> e) =>
        new()
        {
            SignalId = e.Data.AggregateId,
            DataPoints = [
                new AmplificationTrendView.AmplificationDataPoint
                {
                    Timestamp = e.Timestamp,
                    IsAmplified = true,
                },
            ],
        };

    /// <summary>
    /// Creates a new amplification data point view from a signal amplification withdrawn event.
    /// </summary>
    /// <param name="view">The existing amplification trend view to update.</param>
    /// <param name="e">The event.</param>
    public static void Apply(AmplificationTrendView view, IEvent<SignalAmplificationWithdrawn> e)
    {
        view.DataPoints.Add(new AmplificationTrendView.AmplificationDataPoint
        {
            Timestamp = e.Timestamp,
            IsAmplified = false,
        });
    }
}