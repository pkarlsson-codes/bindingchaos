using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Infrastructure.Projections;
using JasperFx.Events.Projections;
using Marten;

namespace BindingChaos.CommunityDiscourse.Infrastructure.Persistence;

/// <summary>
/// Configuration for Marten projections in the CommunityDiscourse bounded context.
/// </summary>
public static class CommunityDiscourseMartenConfiguration
{
    private const string CommunityDiscourseSchemaName = "community_discourse";

    /// <summary>
    /// Configures Marten for the CommunityDiscourse bounded context.
    /// </summary>
    /// <param name="options">The Marten store options to configure.</param>
    public static void Configure(StoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Schema.For<DiscourseThreadView>()
            .DatabaseSchemaName(CommunityDiscourseSchemaName);
        options.Schema.For<ContributionView>()
            .DatabaseSchemaName(CommunityDiscourseSchemaName);

        options.Projections.Add<DiscourseThreadViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<ContributionViewProjection>(ProjectionLifecycle.Async);
    }
}
