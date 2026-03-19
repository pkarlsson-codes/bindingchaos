using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.SharedKernel.Persistence;

/// <summary>
/// Base Marten repository implementation for event-sourced aggregates.
/// </summary>
/// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
/// <typeparam name="TId">The type of the aggregate's unique identifier.</typeparam>
public abstract class MartenRepository<TAggregate, TId> : IRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : EntityId
{
    private static readonly Action<ILogger, string, string, Exception?> LogNoEventsFound =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(1, nameof(LogNoEventsFound)),
            "No events found for aggregate {AggregateType} with id {AggregateId}");

    private static readonly Action<ILogger, string, string, long?, Exception?> LogSuccessfullyLoaded =
        LoggerMessage.Define<string, string, long?>(
            LogLevel.Debug,
            new EventId(2, nameof(LogSuccessfullyLoaded)),
            "Successfully loaded aggregate {AggregateType} with id {AggregateId} from {EventCount} events");

    private static readonly Action<ILogger, string, string, Exception?> LogErrorLoading =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(3, nameof(LogErrorLoading)),
            "Error loading aggregate {AggregateType} with id {AggregateId}");

    private static readonly Action<ILogger, string, string, Exception?> LogNoUncommittedEvents =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(4, nameof(LogNoUncommittedEvents)),
            "No uncommitted events to save for aggregate {AggregateType} with id {AggregateId}");

    private static readonly Action<ILogger, int, string, string, Exception?> LogSuccessfullySaved =
        LoggerMessage.Define<int, string, string>(
            LogLevel.Debug,
            new EventId(5, nameof(LogSuccessfullySaved)),
            "Successfully saved {EventCount} events for aggregate {AggregateType} with id {AggregateId}");

    private static readonly Action<ILogger, string, string, Exception?> LogErrorSaving =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(6, nameof(LogErrorSaving)),
            "Error saving aggregate {AggregateType} with id {AggregateId}");

    private static readonly Action<ILogger, string, string, Exception?> LogErrorFetching =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(7, nameof(LogErrorFetching)),
            "Error checking existence of stream for aggregate {AggregateType} with id {AggregateId}");

    private readonly ILogger<MartenRepository<TAggregate, TId>> _logger;

    /// <summary>
    /// Initializes a new instance of the MartenRepository class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    protected MartenRepository(IDocumentSession session, ILogger<MartenRepository<TAggregate, TId>> logger)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the Marten document session used for database operations.
    /// </summary>
    protected IDocumentSession Session { get; }

    /// <summary>
    /// Asynchronously determines whether a stream with the specified identifier exists.
    /// </summary>
    /// <param name="id">The unique identifier of the stream to check for existence. This parameter cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the stream
    /// exists; otherwise, <see langword="false"/>.</returns>
    public virtual async Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var streamId = GetStreamId(id);

        try
        {
            var stream = await Session.Events.FetchStreamStateAsync(streamId, cancellationToken).ConfigureAwait(false);
            return stream != null;
        }
        catch (Exception ex)
        {
            LogErrorFetching(_logger, typeof(TAggregate).Name, id.ToString(), ex);
            return false;
        }
    }

    /// <summary>
    /// Gets an aggregate by its identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregate if found; otherwise, null.</returns>
    public virtual async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        try
        {
            var streamId = GetStreamId(id);
            var stream = await Session.Events.FetchForWriting<TAggregate>(streamId, cancellationToken).ConfigureAwait(false);

            if (stream.Aggregate is null)
            {
                LogNoEventsFound(_logger, typeof(TAggregate).Name, id.ToString(), null);
                return null;
            }

            LogSuccessfullyLoaded(_logger, typeof(TAggregate).Name, id.ToString(), stream.CurrentVersion, null);
            return stream.Aggregate;
        }
        catch (Exception ex)
        {
            LogErrorLoading(_logger, typeof(TAggregate).Name, id.ToString(), ex);
            throw;
        }
    }

    /// <summary>
    /// Stages an aggregate's uncommitted events to the session without committing changes.
    /// The unit of work should be used to commit the staged changes.
    /// Integration events are published asynchronously via Wolverine event subscriptions after the domain events are committed.
    /// </summary>
    /// <param name="aggregate">The aggregate to stage.</param>
    public virtual void Stage(TAggregate aggregate)
    {
        ArgumentNullException.ThrowIfNull(aggregate);

        aggregate.ValidateInvariants();

        try
        {
            var uncommittedEvents = aggregate.UncommittedEvents.ToArray();

            if (uncommittedEvents.Length == 0)
            {
                LogNoUncommittedEvents(_logger, typeof(TAggregate).Name, aggregate.Id.ToString(), null);
                return;
            }

            var streamId = GetStreamId(aggregate.Id);
            var expectedVersion = aggregate.Version;
            Session.Events.Append(streamId, expectedVersion, uncommittedEvents);

            aggregate.UncommittedEvents.MarkAsCommitted();
            LogSuccessfullySaved(_logger, uncommittedEvents.Length, typeof(TAggregate).Name, aggregate.Id.ToString(), null);
        }
        catch (Exception ex)
        {
            LogErrorSaving(_logger, typeof(TAggregate).Name, aggregate.Id.ToString(), ex);
            throw;
        }
    }

    /// <summary>
    /// Gets an aggregate by its identifier, throwing an exception if not found.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregate.</returns>
    /// <exception cref="AggregateNotFoundException">Thrown when the aggregate is not found.</exception>
    public virtual async Task<TAggregate> GetByIdOrThrowAsync(TId id, CancellationToken cancellationToken = default)
    {
        var aggregate = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        if (aggregate == null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), id);
        }

        return aggregate;
    }

    /// <summary>
    /// Gets the stream identifier for the given aggregate identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <returns>The stream identifier.</returns>
    protected virtual string GetStreamId(TId id)
    {
        return id.Value;
    }

    /// <summary>
    /// Creates a new instance of the aggregate.
    /// This method must be implemented by derived classes to provide the appropriate constructor.
    /// </summary>
    /// <returns>A new instance of the aggregate.</returns>
    protected virtual TAggregate CreateAggregateInstance()
    {
        var constructor = typeof(TAggregate).GetConstructor(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic,
            binder: null,
            types: Type.EmptyTypes,
            modifiers: null) ?? throw new InvalidOperationException($"Type {typeof(TAggregate).Name} must have a parameterless constructor (public or non-public) for event sourcing reconstruction.");
        return (TAggregate)constructor.Invoke(null);
    }
}