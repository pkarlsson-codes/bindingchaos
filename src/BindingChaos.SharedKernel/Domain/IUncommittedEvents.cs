using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Represents a collection of uncommitted domain events.
/// </summary>
public interface IUncommittedEvents : IEnumerable<IDomainEvent>
{
    /// <summary>
    /// Marks all events as committed and clears the collection.
    /// </summary>
    void MarkAsCommitted();
}
