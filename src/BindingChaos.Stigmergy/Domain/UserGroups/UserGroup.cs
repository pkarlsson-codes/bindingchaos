using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Aggregate root representing a user group — a self-organising collection of participants with a shared charter.
/// </summary>
public sealed class UserGroup : AggregateRoot<UserGroupId>
{
    private readonly List<Membership> _members = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    private UserGroup()
    {
        RegisterInvariants();
    }
#pragma warning restore CS8618

    /// <summary>
    /// Gets the charter that governs this group.
    /// </summary>
    public Charter Charter { get; private set; }

    /// <summary>
    /// Gets the participant who founded this group.
    /// </summary>
    public ParticipantId FounderId { get; private set; }

    /// <summary>
    /// Gets the current members of this group.
    /// </summary>
    internal IReadOnlyList<Membership> Members => _members.AsReadOnly();

    /// <summary>
    /// Creates a new user group.
    /// </summary>
    /// <param name="founderId">The participant creating and owning the group.</param>
    /// <param name="commonsId">The ID of the commons this group will govern.</param>
    /// <param name="name">The name of the group.</param>
    /// <param name="charter">The charter that governs the group.</param>
    /// <returns>A new <see cref="UserGroup"/> instance.</returns>
    public static UserGroup Form(ParticipantId founderId, CommonsId commonsId, string name, Charter charter)
    {
        var userGroup = new UserGroup();
        var userGroupId = UserGroupId.Generate();
        var approvalRules = charter.MembershipRules.ApprovalRules is not null
                    ? new MembershipApprovalRulesRecord(
                        charter.MembershipRules.ApprovalRules.ApprovalThreshold,
                        charter.MembershipRules.ApprovalRules.VetoEnabled)
                    : null;
        userGroup.ApplyChange(new UserGroupFormed(userGroupId.Value, commonsId.Value, founderId.Value, name, new CharterRecord(
            new ContentionRulesRecord(charter.ContentionRules.RejectionThreshold, charter.ContentionRules.ResolutionWindow),
            new MembershipRulesRecord(
                charter.MembershipRules.JoinPolicy.Value,
                charter.MembershipRules.MemberListPublic,
                charter.MembershipRules.MaxMembers,
                charter.MembershipRules.EntryRequirements,
                approvalRules),
            new ShunningRulesRecord(charter.ShunningRules.ApprovalThreshold))));

        userGroup.Join(founderId);
        return userGroup;
    }

    /// <summary>
    /// Removes a participant from the user group.
    /// </summary>
    /// <param name="participantId">The ID of the participant leaving the group.</param>
    /// <exception cref="InvalidOperationException">Thrown when the participant is not a member.</exception>
    public void Leave(ParticipantId participantId)
    {
        if (_members.Find(m => m.ParticipantId == participantId) is null)
        {
            throw new InvalidOperationException($"Participant {participantId.Value} is not a member of this group.");
        }

        ApplyChange(new MemberLeft(Id.Value, participantId.Value));
    }

    /// <summary>
    /// Applies for membership in the user group, processing the application according to the group's charter and membership rules, including any necessary validation, state changes, or event generation to reflect the application process and its outcome.
    /// </summary>
    /// <param name="participantId">The ID of the participant applying for membership.</param>
    public void ApplyToJoin(ParticipantId participantId)
    {
        if (Charter.MembershipRules.JoinPolicy == JoinPolicy.Open)
        {
            Join(participantId);
        }
        else
        {
            throw new NotImplementedException("Join policies other than Open are not yet implemented.");
        }
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case UserGroupFormed e: Apply(e); break;
            case MemberJoined e: Apply(e); break;
            case MemberLeft e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent?.GetType().Name}");
        }
    }

    private void Join(ParticipantId participantId)
    {
        ApplyChange(new MemberJoined(Id.Value, MembershipId.Generate().Value, participantId.Value));
    }

    private void Apply(UserGroupFormed e)
    {
        Id = UserGroupId.Create(e.AggregateId);
        FounderId = ParticipantId.Create(e.FounderId);
        var approvalRules = e.Charter.MembershipRules.ApprovalRules is not null
            ? new MembershipApprovalRules(
                e.Charter.MembershipRules.ApprovalRules.ApprovalThreshold,
                e.Charter.MembershipRules.ApprovalRules.VetoEnabled)
            : null;
        Charter = new Charter(
            new ContentionRules(e.Charter.ContentionRules.RejectionThreshold, e.Charter.ContentionRules.ResolutionWindow),
            new MembershipRules(
                Enumeration<JoinPolicy>.FromValue(e.Charter.MembershipRules.JoinPolicy),
                e.Charter.MembershipRules.MemberListPublic,
                e.Charter.MembershipRules.MaxMembers,
                e.Charter.MembershipRules.EntryRequirements,
                approvalRules),
            new ShunningRules(e.Charter.ShunningRules.ApprovalThreshold));
    }

    private void Apply(MemberJoined e)
    {
        _members.Add(new Membership(MembershipId.Create(e.MembershipId), ParticipantId.Create(e.ParticipantId), e.OccurredAt));
    }

    private void Apply(MemberLeft e)
    {
        _members.RemoveAll(m => m.ParticipantId == ParticipantId.Create(e.ParticipantId));
    }

    private void RegisterInvariants()
    {
        AddInvariants(
            MustHaveFounder);
    }

    private void MustHaveFounder()
    {
        if (FounderId == default)
        {
            throw new InvariantViolationException("A user group must have a founder.");
        }
    }
}
