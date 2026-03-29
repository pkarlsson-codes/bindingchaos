using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.Signals;

/// <summary>
/// A <see cref="Signal"/> repository.
/// </summary>
public interface ISignalRepository : IRepository<Signal, SignalId>
{
}