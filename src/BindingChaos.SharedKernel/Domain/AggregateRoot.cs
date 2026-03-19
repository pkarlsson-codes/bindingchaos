using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;

namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Base class for all aggregate roots in the domain.
/// </summary>
/// <typeparam name="TIdentity">The type of the aggregate's unique identifier.</typeparam>
public abstract class AggregateRoot<TIdentity> : Entity<TIdentity>
    where TIdentity : EntityId
{
    private readonly List<Action> _invariants = [];

    private readonly UncommittedEvents _uncommittedEvents = new();

    /// <summary>
    /// Initializes a new instance of the AggregateRoot class.
    /// </summary>
    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Gets the collection of uncommitted domain events for this aggregate.
    /// </summary>
    public IUncommittedEvents UncommittedEvents => _uncommittedEvents;

    /// <summary>
    /// Gets the current version of this aggregate for event sourcing and concurrency control.
    /// </summary>
    public long Version { get; protected set; }

    /// <summary>
    /// Loads the aggregate from a collection of domain events.
    /// This method reconstructs the aggregate state by applying each event in sequence.
    /// </summary>
    /// <param name="events">The collection of domain events to load from.</param>
    public void LoadFromEvents(IEnumerable<IDomainEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        var eventList = events.ToList();
        if (eventList.Count == 0)
        {
            return;
        }

        foreach (var @event in eventList)
        {
            if (@event.Version != Version)
            {
                throw new InvalidOperationException(
                    $"Event version {@event.Version} does not match current aggregate version {Version}. " +
                    $"Events must be applied in sequence.");
            }

            ApplyEvent(@event, false);
        }
    }

    /// <summary>
    /// Validates all invariants for this aggregate.
    /// </summary>
    /// <exception cref="InvariantViolationException">Thrown when any invariant fails validation.</exception>
    public void ValidateInvariants()
    {
        foreach (var invariant in _invariants)
        {
            invariant();
        }
    }

    /// <summary>
    /// Raises a domain event and adds it to the aggregate's event collection.
    /// Automatically increments the aggregate version and passes it to the event.
    /// Also applies the event to update the aggregate state immediately.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void ApplyChange(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        ApplyEvent(domainEvent, true);
    }

    /// <summary>
    /// Adds invariants to the aggregate.
    /// </summary>
    /// <param name="invariants">The invariants to add.</param>
    protected void AddInvariants(params Action[] invariants)
    {
        _invariants.AddRange(invariants);
    }

    /// <summary>
    /// Applies a domain event to this aggregate.
    /// This method must be implemented by derived classes to handle specific event types.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    protected abstract void ApplyEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Applies a domain event to this aggregate.
    /// This method must be implemented by derived classes to handle specific event types.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    /// <param name="isNew">Indicates whether the event is a new event or previously committed event.</param>
    private void ApplyEvent(IDomainEvent domainEvent, bool isNew)
    {
        ApplyEvent(domainEvent);
        Version++;

        if (isNew)
        {
            _uncommittedEvents.Append(domainEvent);
        }
    }
}
