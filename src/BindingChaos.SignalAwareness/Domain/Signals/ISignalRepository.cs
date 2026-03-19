using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Repository interface for Signal aggregate persistence operations.
/// </summary>
public interface ISignalRepository : IRepository<Signal, SignalId>
{
}