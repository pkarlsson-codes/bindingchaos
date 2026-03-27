using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Command to accept an amendment and apply it to the target idea.
/// Only the idea author can accept amendments.
/// </summary>
/// <param name="AmendmentId">The amendment to accept.</param>
/// <param name="ActorId">The participant attempting to accept the amendment.</param>
public sealed record AcceptAmendment(AmendmentId AmendmentId, ParticipantId ActorId);

/// <summary>Handles the <see cref="AcceptAmendment"/> command.</summary>
public static class AcceptAmendmentHandler
{
    /// <summary>Accepts an amendment, applies it to the target idea, and commits the changes.</summary>
    /// <param name="command">The accept amendment command.</param>
    /// <param name="amendmentRepository">Repository for loading and staging the amendment.</param>
    /// <param name="ideaRepository">Repository for loading and staging the target idea.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The identifier of the accepted amendment.</returns>
    public static async Task<AmendmentId> Handle(
        AcceptAmendment command,
        IAmendmentRepository amendmentRepository,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var amendment = await amendmentRepository.GetByIdOrThrowAsync(command.AmendmentId, cancellationToken).ConfigureAwait(false);
        var idea = await ideaRepository.GetByIdOrThrowAsync(amendment.TargetIdeaId, cancellationToken).ConfigureAwait(false);

        if (idea.CreatorId != command.ActorId)
        {
            throw new ForbiddenException($"Only the idea author can accept amendments. Idea author: {idea.CreatorId}, attempting user: {command.ActorId}.");
        }

        if (amendment.TargetVersionNumber != idea.CurrentVersion.VersionNumber)
        {
            throw new BusinessRuleViolationException($"Amendment targets version {amendment.TargetVersionNumber} but idea is at version {idea.CurrentVersion.VersionNumber}.");
        }

        amendment.Accept();
        idea.Amend(amendment.Id, amendment.ProposedTitle, amendment.ProposedBody);

        amendmentRepository.Stage(amendment);
        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return amendment.Id;
    }
}
