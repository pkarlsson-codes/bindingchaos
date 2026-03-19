using BindingChaos.Tagging.Application.ReadModels;
using BindingChaos.Tagging.Infrastructure.Projections;
using JasperFx.Events.Projections;
using Marten;

namespace BindingChaos.Tagging.Infrastructure.Persistence;

/// <summary>
/// Configuration for Marten projections in the Tagging bounded context.
/// </summary>
public static class TaggingMartenConfiguration
{
    private const string TaggingSchemaName = "tagging";

    /// <summary>
    /// Configures Marten for the Tagging bounded context.
    /// </summary>
    /// <param name="options">The Marten store options to configure.</param>
    public static void Configure(StoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Schema.For<TagUsageView>().DatabaseSchemaName(TaggingSchemaName);

        options.Projections.Add<TagUsageProjection>(ProjectionLifecycle.Async);
    }
}
