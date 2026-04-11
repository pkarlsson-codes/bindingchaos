namespace BindingChaos.SharedKernel.Persistence;

/// <summary>
/// Thrown when two concurrent operations both attempt to create the same event stream.
/// Callers that perform idempotent creation should catch this and treat it as success.
/// </summary>
public sealed class ConcurrentStreamCreationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentStreamCreationException"/> class.
    /// </summary>
    /// <param name="streamId">The stream identifier that already exists.</param>
    /// <param name="innerException">The underlying infrastructure exception.</param>
    public ConcurrentStreamCreationException(string streamId, Exception innerException)
        : base($"Stream '{streamId}' was already created by a concurrent operation.", innerException)
    {
        StreamId = streamId;
    }

    /// <summary>Gets the stream identifier that caused the collision.</summary>
    public string StreamId { get; }
}
