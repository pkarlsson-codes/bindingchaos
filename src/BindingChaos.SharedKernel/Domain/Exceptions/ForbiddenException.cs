namespace BindingChaos.SharedKernel.Domain.Exceptions;

/// <summary>
/// Exception thrown when a participant attempts an operation they are not permitted to perform.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
    /// </summary>
    public ForbiddenException()
        : base("You do not have permission to perform this action.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ForbiddenException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
