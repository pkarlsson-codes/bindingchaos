using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons;

/// <summary>
/// Repository interface for Commons aggregate persistence operations.
/// </summary>
public interface ICommonsRepository : IRepository<Commons, CommonsId>
{
}
