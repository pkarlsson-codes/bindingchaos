using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
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

        var amendment = await amendmentRepository.GetByIdAsync(command.AmendmentId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Amendment {command.AmendmentId} not found");

        var idea = await ideaRepository.GetByIdAsync(amendment.TargetIdeaId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Target idea {amendment.TargetIdeaId} not found");

        if (idea.CreatorId != command.ActorId)
        {
            throw new UnauthorizedAccessException(
                $"Only the idea author can accept amendments. " +
                $"Idea author: {idea.CreatorId}, Attempting user: {command.ActorId}");
        }

        if (amendment.TargetVersionNumber != idea.CurrentVersion.VersionNumber)
        {
            throw new InvalidOperationException($"Amendment targets version {amendment.TargetVersionNumber} but idea is at version {idea.CurrentVersion.VersionNumber}");
        }

        amendment.Accept();
        idea.Amend(amendment.Id, amendment.ProposedTitle, amendment.ProposedBody);

        amendmentRepository.Stage(amendment);
        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return amendment.Id;
    }
}
