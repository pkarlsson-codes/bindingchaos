using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Infrastructure.Projections;
using JasperFx.Events.Projections;
using Marten;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Configuration for Marten in the Stigmergy bounded context.
/// </summary>
public static class StigmergyMartenConfiguration
{
    private const string StigmergySchemaName = "stigmergy";

    /// <summary>
    /// Configures Marten for the Stigmergy bounded context.
    /// </summary>
    /// <param name="options">The Marten store options to configure.</param>
    public static void Configure(StoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Schema.For<SignalsListItemView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.CapturedAt)
            .Duplicate(x => x.AmplificationCount)
            .Duplicate(x => x.CapturedById)
            .Duplicate(x => x.Title);

        options.Projections.Add<SignalsListItemViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<SignalView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.CapturedAt)
            .Duplicate(x => x.CapturedById);

        options.Projections.Add<SignalViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<IdeasListItemView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.CreatedAt)
            .Duplicate(x => x.LastUpdatedAt)
            .Duplicate(x => x.AuthorId)
            .Duplicate(x => x.Title)
            .Duplicate(x => x.Status);

        options.Projections.Add<IdeasListItemViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<IdeaView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.CreatedAt)
            .Duplicate(x => x.AuthorId)
            .Duplicate(x => x.Status);

        options.Projections.Add<IdeaViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<AmplificationTrendView>()
            .Identity(x => x.SignalId)
            .DatabaseSchemaName(StigmergySchemaName);

        options.Projections.Add<AmplificationTrendViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<EmergingPatternView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.ClusterLabel)
            .Duplicate(x => x.SignalCount)
            .Duplicate(x => x.LastUpdatedAt);
    }
}
