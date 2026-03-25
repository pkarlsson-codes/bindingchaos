using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons;

/// <summary>
/// The domain of work a <see cref="UserGroups.UserGroup"/> governs — e.g. water management, housing, transportation.
/// A UserGroup governs exactly one Commons; a Commons can have multiple UserGroups, including competing ones with different approaches.
/// </summary>
public sealed class Commons : AggregateRoot<CommonsId>
{
    private Commons()
    {
    }

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

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CommonsCreated e: Apply(e); break;
            default: throw new InvalidOperationException($"Unsupported event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(CommonsCreated e)
    {
        Id = CommonsId.Create(e.AggregateId);
    }
}
