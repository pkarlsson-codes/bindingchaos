using BindingChaos.SignalAwareness.Application.ReadModels;
using BindingChaos.SignalAwareness.Infrastructure.Projections;
using JasperFx.Events.Projections;
using Marten;

namespace BindingChaos.SignalAwareness.Infrastructure.Persistence;

/// <summary>
/// Configuration for Marten projections in the SignalAwareness bounded context.
/// </summary>
public static class SignalAwarenessMartenConfiguration
{
    private const string SignalAwarenessSchemaName = "signal_awareness";

    /// <summary>
    /// Configures Marten for the SignalAwareness bounded context.
    /// </summary>
    /// <param name="options">The Marten store options to configure.</param>
    public static void Configure(StoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Schema.For<SignalView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(SignalAwarenessSchemaName)
            .Duplicate(x => x.CapturedAt)
            .Duplicate(x => x.AmplificationCount)
            .Duplicate(x => x.Status)
            .Duplicate(x => x.OriginatorId)
            .Duplicate(x => x.Title);
        options.Schema.For<SignalsListItemView>().DatabaseSchemaName(SignalAwarenessSchemaName);
        options.Schema.For<SignalAmplificationTrendView>().DatabaseSchemaName(SignalAwarenessSchemaName);

        options.Projections.Add<SignalViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<SignalsListItemViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<SignalAmplificationTrendViewProjection>(ProjectionLifecycle.Async);
    }
}
