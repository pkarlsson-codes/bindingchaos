using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.SharedKernel.Infrastructure.Services;

/// <summary>
/// Default implementation of ITimeProvider that returns the current system time.
/// </summary>
public sealed class SystemTimeProvider : ITimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}