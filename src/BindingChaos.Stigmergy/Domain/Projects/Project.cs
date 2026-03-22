using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Projects.Events;
using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// Aggregate root representing a stigmergic project — a coordinated work effort
/// initiated by a user group and driven by pledges against resource requirements.
/// </summary>
public sealed class Project : AggregateRoot<ProjectId>
{
    private readonly List<Amendment> _amendments = [];

    private readonly List<Requirement> _requirements = [];

#pragma warning disable CS8618
    private Project()
    {
        RegisterInvariants();
    }
#pragma warning restore CS8618

    /// <summary>Gets the title of this project.</summary>
    public string Title { get; private set; }

    /// <summary>Gets the description of this project.</summary>
    public string Description { get; private set; }

    /// <summary>Gets the identifier of the user group that owns this project.</summary>
    public UserGroupId UserGroupId { get; private set; }

    /// <summary>Gets the amendments proposed against this project.</summary>
    public IReadOnlyList<Amendment> Amendments => _amendments.AsReadOnly();

    /// <summary>Gets the resource requirements for this project.</summary>
    public IReadOnlyList<Requirement> Requirements => _requirements.AsReadOnly();

    /// <summary>
    /// Creates a new project for the specified user group.
    /// </summary>
    /// <param name="userGroupId">The identifier of the user group initiating this project.</param>
    /// <param name="title">The title of the project.</param>
    /// <param name="description">The description of the project.</param>
    /// <returns>A new <see cref="Project"/> instance.</returns>
    public static Project Create(UserGroupId userGroupId, string title, string description)
    {
        var project = new Project();
        var projectId = ProjectId.Generate();
        project.ApplyChange(new ProjectCreated(projectId.Value, userGroupId.Value, title, description));
        return project;
    }

    /// <summary>
    /// Proposes a new amendment to this project, immediately set to Active.
    /// </summary>
    /// <param name="proposedBy">The participant proposing the amendment.</param>
    /// <returns>The identifier of the newly proposed amendment.</returns>
    public AmendmentId ProposeAmendment(ParticipantId proposedBy)
    {
        var amendmentId = AmendmentId.Generate();
        ApplyChange(new AmendmentProposed(
            Id.Value,
            Version,
            amendmentId.Value,
            proposedBy.Value,
            SharedKernel.Domain.Services.TimeProviderContext.Current.UtcNow));
        return amendmentId;
    }

    /// <summary>
    /// Applies a domain event to this aggregate, updating its state.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ProjectCreated e: Apply(e); break;
            case AmendmentProposed e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent?.GetType().Name}");
        }
    }

    private void Apply(ProjectCreated e)
    {
        Id = ProjectId.Create(e.AggregateId);
        UserGroupId = UserGroupId.Create(e.UserGroupId);
        Title = e.Title;
        Description = e.Description;
    }

    private void Apply(AmendmentProposed e)
    {
        _amendments.Add(new Amendment(
            AmendmentId.Create(e.AmendmentId),
            ParticipantId.Create(e.ProposedBy),
            e.ProposedAt,
            AmendmentStatus.Active));
    }

    private void RegisterInvariants()
    {
        AddInvariants();
    }
}
