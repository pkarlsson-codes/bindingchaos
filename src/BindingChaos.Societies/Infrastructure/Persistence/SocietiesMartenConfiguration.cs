using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Infrastructure.Projections;
using JasperFx.Events.Projections;
using Marten;

namespace BindingChaos.Societies.Infrastructure.Persistence;

/// <summary>
/// Configuration for Marten projections and read models in the Societies bounded context.
/// </summary>
public static class SocietiesMartenConfiguration
{
    private const string SocietiesSchemaName = "societies";

    /// <summary>
    /// Configures Marten for the Societies bounded context.
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
        options.Projections.Add<SocietyViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<SocietyListItemViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<SocietyMemberViewProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<SocialContractViewProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<SocietyAffectedByCommonsViewProjection>(ProjectionLifecycle.Inline);
    }

    private static void ConfigureReadModels(StoreOptions options)
    {
        options.Schema.For<SocietyView>()
            .DatabaseSchemaName(SocietiesSchemaName);
        options.Schema.For<SocietyListItemView>()
            .DatabaseSchemaName(SocietiesSchemaName);
        options.Schema.For<SocietyMemberView>()
            .DatabaseSchemaName(SocietiesSchemaName);
        options.Schema.For<SocialContractView>()
            .DatabaseSchemaName(SocietiesSchemaName)
            .Index(v => v.SocietyId);
        options.Schema.For<SocietyAffectedByCommonsView>()
            .DatabaseSchemaName(SocietiesSchemaName)
            .Duplicate(x => x.CommonsId)
            .Duplicate(x => x.SocietyId);
    }
}
