using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.SignalAwareness.Domain.Evidence;

/// <summary>
/// Defines a contract for managing and accessing evidence entities within a repository pattern implementation.
/// </summary>
public interface IEvidenceRepository : IRepository<Evidence, EvidenceId>
{
}