using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.Ideation.Domain.Ideas.Events;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.EventHandlers;

/// <summary>
/// Event handler that marks all amendments targeting an idea as outdated when the idea is amended.
/// </summary>
public static class MarkAmendmentsOutdated
{
    /// <summary>
    /// Handles marking all amendments targeting an idea as outdated when the idea is amended.
    /// </summary>
    /// <param name="notification">The idea amended event.</param>
    /// <param name="amendmentRepository">Repository for amendment aggregates.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        IdeaAmended notification,
        IAmendmentRepository amendmentRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var ideaId = IdeaId.Create(notification.AggregateId);
        var newVersionNumber = notification.NewVersionNumber;

        var openAmendments = await amendmentRepository.GetOpenAmendmentsForIdeaAsync(ideaId, cancellationToken).ConfigureAwait(false);

        foreach (var amendment in openAmendments)
        {
            if (amendment.TargetVersionNumber < newVersionNumber)
            {
                amendment.MarkOutdated(newVersionNumber);
                amendmentRepository.Stage(amendment);
            }
        }

        if (openAmendments.Any(a => a.TargetVersionNumber < newVersionNumber))
        {
            await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
