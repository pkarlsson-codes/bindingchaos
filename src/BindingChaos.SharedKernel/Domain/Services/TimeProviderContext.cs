using BindingChaos.SharedKernel.Infrastructure.Services;

namespace BindingChaos.SharedKernel.Domain.Services;

/// <summary>
/// Global context for accessing the current time provider.
/// This allows domain events to get the current time without direct dependency injection.
/// </summary>
public static class TimeProviderContext
{
    private static ITimeProvider _current = new SystemTimeProvider();

    /// <summary>
    /// Gets the current time provider instance.
    /// </summary>
    public static ITimeProvider Current => _current;

    /// <summary>
    /// Sets the current time provider. This should typically only be called during
    /// application startup, testing, or seeding scenarios.
    /// </summary>
    /// <param name="timeProvider">The time provider to set as current.</param>
    /// <exception cref="ArgumentNullException">Thrown when timeProvider is null.</exception>
    public static void SetCurrent(ITimeProvider timeProvider)
    {
        _current = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    /// <summary>
    /// Resets the time provider to the default system time provider.
    /// </summary>
    public static void Reset()
    {
        _current = new SystemTimeProvider();
    }
}