namespace BindingChaos.SharedKernel.Domain.Exceptions;

/// <summary>
/// Exception thrown when an aggregate root is not found in the repository.
/// </summary>
public class AggregateNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the AggregateNotFoundException class.
    /// </summary>
    /// <param name="aggregateType">The type of the aggregate that was not found.</param>
    /// <param name="aggregateId">The identifier of the aggregate that was not found.</param>
    public AggregateNotFoundException(Type aggregateType, object aggregateId)
        : base($"Aggregate {aggregateType.Name} with id {aggregateId} was not found.")
    {
        AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));
        AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
    }

    /// <summary>
    /// Initializes a new instance of the AggregateNotFoundException class with a custom message.
    /// </summary>
    /// <param name="aggregateType">The type of the aggregate that was not found.</param>
    /// <param name="aggregateId">The identifier of the aggregate that was not found.</param>
    /// <param name="message">The custom error message.</param>
    public AggregateNotFoundException(Type aggregateType, object aggregateId, string message)
        : base(message)
    {
        AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));
        AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
    }

    /// <summary>
    /// Initializes a new instance of the AggregateNotFoundException class.
    /// </summary>
    public AggregateNotFoundException()
    {
        AggregateType = typeof(object);
        AggregateId = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the AggregateNotFoundException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AggregateNotFoundException(string message)
        : base(message)
    {
        AggregateType = typeof(object);
        AggregateId = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the AggregateNotFoundException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AggregateNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
        AggregateType = typeof(object);
        AggregateId = string.Empty;
    }

    /// <summary>
    /// Gets the type of the aggregate that was not found.
    /// </summary>
    public Type AggregateType { get; }

    /// <summary>
    /// Gets the identifier of the aggregate that was not found.
    /// </summary>
    public object AggregateId { get; }
}