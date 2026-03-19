using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads.Events;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;

namespace BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;

/// <summary>
/// Aggregate root representing a discourse thread for community discussion around an entity.
/// </summary>
public sealed class DiscourseThread : AggregateRoot<DiscourseThreadId>
{
    /// <summary>
    /// Initializes a new instance of the DiscourseThread class.
    /// For use by event sourcing reconstruction.
    /// </summary>
    private DiscourseThread()
    {
        RegisterInvariants();
    }

    /// <summary>
    /// Initializes a new instance of the DiscourseThread class.
    /// </summary>
    /// <param name="entityReference">Reference to the entity this discourse is about.</param>
    private DiscourseThread(EntityReference entityReference)
    {
        RegisterInvariants();

        ApplyChange(new DiscourseThreadCreated(
            DiscourseThreadId.Generate().Value,
            Version,
            entityReference.EntityType,
            entityReference.EntityId));
    }

    /// <summary>
    /// Gets the reference to the entity this discourse is about.
    /// </summary>
    public EntityReference EntityReference { get; private set; } = null!;

    /// <summary>
    /// Gets the timestamp when the thread was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Creates a new discourse thread for an entity.
    /// </summary>
    /// <param name="entityReference">Reference to the entity this discourse is about.</param>
    /// <returns>A new DiscourseThread instance.</returns>
    public static DiscourseThread Create(EntityReference entityReference)
    {
        return new DiscourseThread(entityReference);
    }

    /// <summary>
    /// Applies a domain event to mutate the aggregate state.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        switch (domainEvent)
        {
            case DiscourseThreadCreated e:
                Apply(e);
                break;
            default:
                throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    /// <summary>
    /// Registers invariants for this aggregate.
    /// </summary>
    private void RegisterInvariants()
    {
        AddInvariants(
            EntityReferenceMustBeSpecified);
    }

    /// <summary>
    /// Applies the DiscourseThreadCreated event to the aggregate state.
    /// </summary>
    /// <param name="e">The event to apply.</param>
    private void Apply(DiscourseThreadCreated e)
    {
        Id = DiscourseThreadId.Create(e.AggregateId);
        var entityReference = EntityReference.Create(e.EntityType, e.EntityId);
        EntityReference = entityReference;
        CreatedAt = e.OccurredAt;
    }

    #region Invariants

    /// <summary>
    /// Invariant: entity reference must be specified.
    /// </summary>
    private void EntityReferenceMustBeSpecified()
    {
        if (EntityReference == null)
        {
            throw new InvariantViolationException("Entity reference cannot be null");
        }
    }

    #endregion
}