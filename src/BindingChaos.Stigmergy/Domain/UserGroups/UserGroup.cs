using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;

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
    /// Gets the name of the user group.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the identifier of the participant who owns this group.
    /// </summary>
    public ParticipantId OwnerId { get; private set; }

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
    /// <param name="ownerId">The participant creating and owning the group.</param>
    /// <param name="name">The name of the group.</param>
    /// <param name="charter">The charter that governs the group.</param>
    /// <returns>A new <see cref="UserGroup"/> instance.</returns>
    public static UserGroup Create(ParticipantId ownerId, string name, Charter charter)
    {
        throw new NotImplementedException("No domain events have been defined for UserGroup yet.");
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent?.GetType().Name}");
        }
    }

    private void RegisterInvariants()
    {
        AddInvariants();
    }
}
