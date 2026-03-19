namespace BindingChaos.SharedKernel.Persistence;

/// <summary>
/// Unit of work interface for managing transactions and ensuring consistency across multiple repository operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}