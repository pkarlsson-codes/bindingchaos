using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Represents a command to propose an amendment to an existing idea.
/// </summary>
/// <param name="IdeaId">The unique identifier of the idea to which the amendment is being proposed.</param>
/// <param name="TargetIdeaVersion">The version of the idea that the amendment targets. Must match the current version of the idea.</param>
/// <param name="ProposerId">The unique identifier of the participant proposing the amendment.</param>
/// <param name="Title">The title of the proposed amendment. Cannot be null or empty.</param>
/// <param name="Body">The detailed description or content of the proposed amendment. Cannot be null or empty.</param>
/// <param name="AmendmentTitle">The title of the amendment. Cannot be null or empty.</param>
/// <param name="AmendmentDescription">The description of the amendment. Cannot be null or empty.</param>
public sealed record ProposeAmendment(
    IdeaId IdeaId,
    int TargetIdeaVersion,
    ParticipantId ProposerId,
    string Title,
    string Body,
    string AmendmentTitle,
    string AmendmentDescription);

/// <summary>
/// Handles the <see cref="ProposeAmendment"/> command to propose an amendment to a signal.
/// </summary>
public static class ProposeAmendmentHandler
{
    /// <summary>Handles the <see cref="ProposeAmendment"/> command by creating and persisting a new Amendment aggregate.</summary>
    /// <param name="request">The propose amendment command.</param>
    /// <param name="amendmentRepository">Repository for staging the proposed amendment.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The identifier of the proposed amendment.</returns>
    public static async Task<AmendmentId> Handle(
        ProposeAmendment request,
        IAmendmentRepository amendmentRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var proposedAmendment = Amendment.Propose(
            request.IdeaId,
            request.TargetIdeaVersion,
            request.ProposerId,
            request.Title,
            request.Body,
            request.AmendmentTitle,
            request.AmendmentDescription);

        amendmentRepository.Stage(proposedAmendment);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return proposedAmendment.Id;
    }
}
