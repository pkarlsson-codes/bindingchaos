using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Domain.Amendments;

/// <summary>
/// Repository interface for the Amendment aggregate.
/// </summary>
public interface IAmendmentRepository : IRepository<Amendment, AmendmentId>
{
    /// <summary>
    /// Gets all open amendments targeting a specific idea.
    /// </summary>
    /// <param name="ideaId">The ID of the idea.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of open amendments targeting the idea.</returns>
    Task<IEnumerable<Amendment>> GetOpenAmendmentsForIdeaAsync(IdeaId ideaId, CancellationToken cancellationToken = default);
}
