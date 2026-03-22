using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Base record for all domain events.
/// </summary>
/// <param name="AggregateId">The strongly-typed ID of the aggregate that raised this event.</param>
public abstract record DomainEvent(string AggregateId) : IDomainEvent
{
    /// <summary>
    /// Gets the timestamp when this domain event occurred.
    /// </summary>
    public DateTimeOffset OccurredAt { get; init; } = TimeProviderContext.Current.UtcNow;

    /// <inheritdoc />
    public override string ToString() => $"{GetType().Name}[{AggregateId}]";
}