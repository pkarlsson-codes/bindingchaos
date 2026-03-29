using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.Concerns;

/// <summary>
/// A <see cref="Concern"/> repository.
/// </summary>
public interface IConcernRepository : IRepository<Concern, ConcernId>
{
}