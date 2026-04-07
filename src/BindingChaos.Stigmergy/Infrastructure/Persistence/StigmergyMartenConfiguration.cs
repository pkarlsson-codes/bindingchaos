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
            .Duplicate(x => x.Title)
            .Index(x => x.AmplifierIds, x => x.Method = Weasel.Postgresql.Tables.IndexMethod.gin);

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

        options.Schema.For<ConcernsListItemView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.RaisedById);

        options.Projections.Add<ConcernsListItemViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<CommonsListItemView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.FounderId)
            .Duplicate(x => x.Status)
            .Duplicate(x => x.ProposedAt);

        options.Projections.Add<CommonsListItemViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<UserGroupListItemView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.CommonsId)
            .Duplicate(x => x.FounderId)
            .Duplicate(x => x.FormedAt)
            .Duplicate(x => x.MemberCount);

        options.Projections.Add<UserGroupListItemViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<ProjectsListItemView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.UserGroupId)
            .Duplicate(x => x.CreatedAt)
            .Duplicate(x => x.LastUpdatedAt)
            .Duplicate(x => x.Title)
            .Duplicate(x => x.ActiveAmendmentCount)
            .Duplicate(x => x.ContestedAmendmentCount)
            .Duplicate(x => x.RejectedAmendmentCount);

        options.Projections.Add<ProjectsListItemViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<ProjectView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.UserGroupId)
            .Duplicate(x => x.CreatedAt)
            .Duplicate(x => x.LastUpdatedAt);

        options.Projections.Add<ProjectViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<ResourceRequirementView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.ProjectId)
            .Duplicate(x => x.CreatedAt)
            .Duplicate(x => x.LastUpdatedAt);

        options.Projections.Add<ResourceRequirementViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<SignalAmplificationsView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.SignalId)
            .Duplicate(x => x.OccurredAt);

        options.Projections.Add<SignalAmplificationsViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<UserGroupMembersView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.UserGroupId)
            .Duplicate(x => x.JoinedAt);

        options.Projections.Add<UserGroupMembersViewProjection>(ProjectionLifecycle.Async);

        options.Schema.For<ConcernAffectedParticipantsView>()
            .Identity(x => x.Id)
            .DatabaseSchemaName(StigmergySchemaName)
            .Duplicate(x => x.ConcernId)
            .Duplicate(x => x.IndicatedAt);

        options.Projections.Add<ConcernAffectedParticipantsViewProjection>(ProjectionLifecycle.Async);
    }
}
