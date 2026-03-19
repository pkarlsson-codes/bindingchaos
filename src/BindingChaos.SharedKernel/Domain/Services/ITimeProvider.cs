namespace BindingChaos.SharedKernel.Domain.Services;

/// <summary>
/// Abstraction for providing the current time.
/// This allows for controllable time in tests and development scenarios.
/// </summary>
public interface ITimeProvider
{
    /// <summary>
    /// Gets the current date and time as a DateTimeOffset in UTC.
    /// </summary>
    /// <returns>The current UTC DateTimeOffset.</returns>
    DateTimeOffset UtcNow { get; }
}