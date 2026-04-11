using Marten;
using Marten.Exceptions;
using Microsoft.Extensions.Logging;

namespace BindingChaos.SharedKernel.Persistence;

/// <summary>
/// Marten unit of work implementation that manages transactions using Marten's document session.
/// </summary>
public partial class MartenUnitOfWork : IUnitOfWork
{
    private readonly IDocumentSession _session;
    private readonly ILogger<MartenUnitOfWork> _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MartenUnitOfWork class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    public MartenUnitOfWork(IDocumentSession session, ILogger<MartenUnitOfWork> logger)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            Logs.TransactionCommitted(_logger);
        }
        catch (ExistingStreamIdCollisionException ex)
        {
            Logs.ErrorCommittingTransaction(_logger, ex);
            throw new ConcurrentStreamCreationException(ex.Id.ToString()!, ex);
        }
        catch (Exception ex)
        {
            Logs.ErrorCommittingTransaction(_logger, ex);
            throw;
        }
    }

    /// <summary>
    /// Disposes the unit of work and underlying session.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work and underlying session.
    /// </summary>
    /// <param name="disposing">True if disposing; false if finalizing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _session?.Dispose();
            _disposed = true;
        }
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Transaction committed")]
        internal static partial void TransactionCommitted(ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error committing transaction")]
        internal static partial void ErrorCommittingTransaction(ILogger logger, Exception? exception);
    }
}
