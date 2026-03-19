namespace BindingChaos.SharedKernel.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated during domain operations.
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class.
    /// </summary>
    public BusinessRuleViolationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BusinessRuleViolationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BusinessRuleViolationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
