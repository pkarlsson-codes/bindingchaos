using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.Ideas;

/// <summary>
/// An <see cref="Idea"/> repository.
/// </summary>
public interface IIdeaRepository : IRepository<Idea, IdeaId>
{
}