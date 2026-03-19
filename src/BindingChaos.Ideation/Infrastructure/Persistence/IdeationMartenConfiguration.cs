using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Infrastructure.Projections;
using JasperFx.Events.Projections;
using Marten;

namespace BindingChaos.Ideation.Infrastructure.Persistence;

/// <summary>
/// Configuration for Marten projections in the Ideation bounded context.
/// </summary>
public static class IdeationMartenConfiguration
{
    private const string IdeationSchemaName = "ideation";

    /// <summary>
    /// Configures Marten for the Ideation bounded context.
    /// </summary>
    /// <param name="options">The Marten store options to configure.</param>
    public static void Configure(StoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        ConfigureReadModels(options);
        ConfigureProjections(options);
    }

    private static void ConfigureProjections(StoreOptions options)
    {
        options.Projections.Add<IdeaViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<IdeasListItemViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<AmendmentsListItemViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<AmendmentDetailViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<AmendmentSupporterViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<AmendmentOpponentViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<AmendmentTrendViewProjection>(ProjectionLifecycle.Async);
    }

    private static void ConfigureReadModels(StoreOptions options)
    {
        options.Schema.For<IdeaView>()
            .DatabaseSchemaName(IdeationSchemaName);
        options.Schema.For<IdeasListItemView>()
            .DatabaseSchemaName(IdeationSchemaName);
        options.Schema.For<AmendmentsListItemView>()
            .DatabaseSchemaName(IdeationSchemaName);
        options.Schema.For<AmendmentDetailView>()
            .DatabaseSchemaName(IdeationSchemaName);
        options.Schema.For<AmendmentSupporterView>()
            .DatabaseSchemaName(IdeationSchemaName);
        options.Schema.For<AmendmentOpponentView>()
            .DatabaseSchemaName(IdeationSchemaName);
        options.Schema.For<AmendmentTrendView>()
            .DatabaseSchemaName(IdeationSchemaName);
    }
}
