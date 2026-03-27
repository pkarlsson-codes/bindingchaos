using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;

namespace BindingChaos.SharedKernel.Persistence;

/// <summary>
/// Base repository interface for event-sourced aggregates.
/// </summary>
/// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
/// <typeparam name="TId">The type of the aggregate's unique identifier.</typeparam>
public interface IRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : EntityId
{
    /// <summary>
    /// Asynchronously determines whether an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to check for existence.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if an entity
    /// with the specified identifier exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an aggregate by its identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregate if found; otherwise, null.</returns>
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages an aggregate's uncommitted events to the session without committing changes.
    /// The unit of work should be used to commit the staged changes.
    /// </summary>
    /// <param name="aggregate">The aggregate to stage.</param>
    void Stage(TAggregate aggregate);

    /// <summary>
    /// Gets an aggregate by its identifier, throwing an exception if not found.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregate.</returns>
    /// <exception cref="AggregateNotFoundException">Thrown when the aggregate is not found.</exception>
    Task<TAggregate> GetByIdOrThrowAsync(TId id, CancellationToken cancellationToken = default);
}
