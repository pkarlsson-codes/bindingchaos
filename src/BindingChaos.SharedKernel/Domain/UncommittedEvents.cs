using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Maintains a collection of domain events that have been raised but not yet committed.
/// </summary>
internal class UncommittedEvents : IUncommittedEvents
{
    private readonly List<IDomainEvent> _events = [];

    /// <summary>
    /// Appends a domain event to the uncommitted collection.
    /// </summary>
    /// <param name="eventToAdd">The domain event to append.</param>
    public void Append(IDomainEvent eventToAdd)
    {
        ArgumentNullException.ThrowIfNull(eventToAdd);
        _events.Add(eventToAdd);
    }

    /// <summary>
    /// Marks all currently uncommitted events as committed by clearing the collection.
    /// </summary>
    public void MarkAsCommitted() { _events.Clear(); }

    /// <summary>
    /// Returns an enumerator that iterates through the uncommitted domain events.
    /// </summary>
    /// <returns>An enumerator over the uncommitted domain events.</returns>
    public IEnumerator<IDomainEvent> GetEnumerator() { return _events.GetEnumerator(); }

    /// <summary>
    /// Returns a non-generic enumerator that iterates through the uncommitted domain events.
    /// </summary>
    /// <returns>An enumerator over the uncommitted domain events.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
}