using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SharedKernel.Persistence;

/// <summary>
/// Base interface for Marten projections that handle domain events.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public interface IProjection<TReadModel>
{
    /// <summary>
    /// Gets the name of the projection.
    /// </summary>
    string ProjectionName { get; }

    /// <summary>
    /// Handles a domain event and updates the read model accordingly.
    /// </summary>
    /// <param name="readModel">The current read model state.</param>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <returns>The updated read model.</returns>
    TReadModel Handle(TReadModel readModel, IDomainEvent domainEvent);

    /// <summary>
    /// Creates a new read model instance.
    /// </summary>
    /// <returns>A new read model instance.</returns>
    TReadModel Create();
}