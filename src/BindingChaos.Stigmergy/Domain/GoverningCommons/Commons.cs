using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using Spectre.Console;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons;

/// <summary>
/// The domain of work a <see cref="UserGroups.UserGroup"/> governs — e.g. water management, housing, transportation.
/// A UserGroup governs exactly one Commons; a Commons can have multiple UserGroups, including competing ones with different approaches.
/// </summary>
public sealed class Commons : AggregateRoot<CommonsId>
{
    private CommonsStatus _status;
    private string _name = string.Empty;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Commons() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Proposes a new <see cref="Commons"/>.
    /// </summary>
    /// <param name="name">Name of the commons.</param>
    /// <param name="description">Description of the commons.</param>
    /// <param name="founderId">The participant proposing the commons.</param>
    /// <returns>The created <see cref="Commons"/>.</returns>
    public static Commons Propose(string name, string description, ParticipantId founderId)
    {
        var commons = new Commons();
        var commonsId = CommonsId.Generate();
        commons.ApplyChange(new CommonsCreated(commonsId.Value, name, description, founderId.Value));
        return commons;
    }

    /// <summary>
    /// Renames the commons.
    /// </summary>
    /// <param name="newName">The new name for the commons.</param>
    public void Rename(string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        ApplyChange(new CommonsRenamed(Id.Value, newName));
    }

    /// <summary>
    /// Activates the commons, allowing it to be governed by user groups. This is called when the first user group is formed to govern this commons.
    /// </summary>
    internal void Activate()
    {
        if (_status != CommonsStatus.Proposed)
        {
            throw new BusinessRuleViolationException("Only proposed commons can be activated.");
        }

        ApplyChange(new CommonsActivated(Id.Value));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CommonsCreated e: Apply(e); break;
            case CommonsActivated e: Apply(e); break;
            case CommonsRenamed e: Apply(e); break;
            default: throw new InvalidOperationException($"Unsupported event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(CommonsCreated e)
    {
        Id = CommonsId.Create(e.AggregateId);
        _name = e.Name;
        _status = CommonsStatus.Proposed;
    }

    private void Apply(CommonsActivated e)
    {
        _status = CommonsStatus.Active;
    }

    private void Apply(CommonsRenamed e)
    {
        _name = e.NewName;
    }
}
