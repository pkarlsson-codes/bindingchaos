namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Base interface for domain event handlers.
/// </summary>
/// <typeparam name="TEvent">The type of domain event to handle.</typeparam>
public interface IHandleDomainEvents<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent domainEvent);
}

/// <summary>
/// Non-generic interface for event handlers that can handle any domain event.
/// </summary>
public interface IHandleDomainEvents
{
    /// <summary>
    /// Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(IDomainEvent domainEvent);
}