using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Repository interface for the Idea aggregate.
/// </summary>
public interface IIdeaRepository : IRepository<Idea, IdeaId>
{
}
