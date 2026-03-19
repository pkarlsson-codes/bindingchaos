using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Base record for all domain events.
/// </summary>
/// <param name="AggregateId">The strongly-typed ID of the aggregate that raised this event.</param>
/// <param name="Version">The version of the aggregate when this event was raised.</param>
public abstract record DomainEvent(string AggregateId, long Version) : IDomainEvent
{
    /// <summary>
    /// Gets the timestamp when this domain event occurred.
    /// </summary>
    public DateTimeOffset OccurredAt { get; init; } = TimeProviderContext.Current.UtcNow;

    /// <inheritdoc />
    public override string ToString() => $"{GetType().Name}[{AggregateId}]";
}