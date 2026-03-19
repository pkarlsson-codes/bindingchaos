using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.SharedKernel.Infrastructure.Services;

/// <summary>
/// Implementation of ITimeProvider that allows manual control of time progression for seeding and testing.
/// </summary>
public sealed class ControllableTimeProvider : ITimeProvider
{
    private DateTimeOffset _currentTime;

    /// <summary>
    /// Initializes a new instance of the ControllableTimeProvider class.
    /// </summary>
    /// <param name="startTime">The initial time to start from.</param>
    public ControllableTimeProvider(DateTimeOffset startTime)
    {
        _currentTime = startTime;
    }

    /// <inheritdoc />
    public DateTimeOffset UtcNow => _currentTime;

    /// <summary>
    /// Advances the current time by the specified duration.
    /// </summary>
    /// <param name="duration">The duration to advance the time by.</param>
    public void Advance(TimeSpan duration)
    {
        _currentTime = _currentTime.Add(duration);
    }

    /// <summary>
    /// Advances the current time by the specified number of days.
    /// </summary>
    /// <param name="days">The number of days to advance.</param>
    public void AdvanceDays(int days)
    {
        _currentTime = _currentTime.AddDays(days);
    }

    /// <summary>
    /// Advances the current time by the specified number of hours.
    /// </summary>
    /// <param name="hours">The number of hours to advance.</param>
    public void AdvanceHours(int hours)
    {
        _currentTime = _currentTime.AddHours(hours);
    }

    /// <summary>
    /// Sets the current time to a specific value.
    /// </summary>
    /// <param name="dateTime">The time to set as current.</param>
    public void SetTime(DateTimeOffset dateTime)
    {
        _currentTime = dateTime;
    }
}