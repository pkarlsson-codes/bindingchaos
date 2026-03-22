using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    /// Gets the current members of this group.
    /// </summary>
    internal IReadOnlyList<Membership> Members => _members.AsReadOnly();

    /// <summary>
    /// Creates a new user group.
    /// </summary>
    /// <param name="founderId">The participant creating and owning the group.</param>
    /// <param name="name">The name of the group.</param>
    /// <param name="charter">The charter that governs the group.</param>
    /// <returns>A new <see cref="UserGroup"/> instance.</returns>
    public static UserGroup Create(ParticipantId founderId, string name, Charter charter)
    {
        var userGroup = new UserGroup();
        var approvalRules = charter.MembershipRules.ApprovalRules is not null
                    ? new MembershipApprovalRulesRecord(
                        charter.MembershipRules.ApprovalRules.ApprovalThreshold,
                        charter.MembershipRules.ApprovalRules.VetoEnabled)
                    : null;
        userGroup.ApplyChange(new UserGroupCreated(userGroup.Id.Value, founderId.Value, name, new CharterRecord(
            new ContentionRulesRecord(charter.ContentionRules.RejectionThreshold, charter.ContentionRules.ResolutionWindow),
            new MembershipRulesRecord(
                charter.MembershipRules.JoinPolicy.Value,
                charter.MembershipRules.MemberListPublic,
                charter.MembershipRules.MaxMembers,
                charter.MembershipRules.EntryRequirements,
                approvalRules),
            new ShunningRulesRecord(charter.ShunningRules.ApprovalThreshold))));
        return userGroup;
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case UserGroupCreated e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent?.GetType().Name}");
        }
    }

    private void Apply(UserGroupCreated @event)
    {
        Id = UserGroupId.Create(@event.UserGroupId);
    }

    private void RegisterInvariants()
    {
        AddInvariants();
    }
}
