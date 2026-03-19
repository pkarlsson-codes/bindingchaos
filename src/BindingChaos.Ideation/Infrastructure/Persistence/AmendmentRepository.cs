using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Persistence;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Ideation.Infrastructure.Persistence;

/// <summary>
/// Repository for managing amendments.
/// </summary>
internal class AmendmentRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Amendment, AmendmentId>> logger)
    : MartenRepository<Amendment, AmendmentId>(session, logger), IAmendmentRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Amendment>> GetOpenAmendmentsForIdeaAsync(IdeaId ideaId, CancellationToken cancellationToken = default)
    {
        var amendmentViews = await Session.Query<AmendmentsListItemView>()
            .Where(x => x.IdeaId == ideaId.Value && x.Status == AmendmentStatus.Open.Value)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var amendmentIds = amendmentViews.Select(v => AmendmentId.Create(v.Id)).ToArray();
        var tasks = amendmentIds.Select(id => GetByIdAsync(id, cancellationToken));
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        return results.Where(a => a != null)!;
    }
}