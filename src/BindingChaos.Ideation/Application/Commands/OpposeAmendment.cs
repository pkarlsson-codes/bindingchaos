using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Represents a command to oppose an amendment.
/// </summary>
/// <param name="AmendmentId">The unique identifier of the amendment to oppose.</param>
/// <param name="ParticipantId">The unique identifier of the participant opposing the amendment.</param>
/// <param name="Reason">The reason provided by the participant for opposing the amendment.</param>
public sealed record OpposeAmendment(
    AmendmentId AmendmentId,
    ParticipantId ParticipantId,
    string Reason);

/// <summary>Handles the <see cref="OpposeAmendment"/> command to add opposition to an amendment.</summary>
public static class OpposeAmendmentHandler
{
    /// <summary>Adds a participant's opposition to an amendment and returns the updated vote counts.</summary>
    /// <param name="request">The oppose amendment command.</param>
    /// <param name="amendmentRepository">Repository for loading and staging the amendment.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts.</returns>
    public static async Task<AmendmentSupportCounts> Handle(
        OpposeAmendment request,
        IAmendmentRepository amendmentRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var amendment = await amendmentRepository.GetByIdAsync(request.AmendmentId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Amendment {request.AmendmentId} not found");

        if (amendment.Supporters.Any(s => s.ParticipantId == request.ParticipantId))
        {
            throw new BusinessRuleViolationException("Participant must withdraw support before opposing this amendment.");
        }

        var opponent = new Opponent(request.ParticipantId, request.Reason);
        amendment.AddOpposition(opponent);
        amendmentRepository.Stage(amendment);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return new AmendmentSupportCounts(amendment.Supporters.Count, amendment.Opponents.Count);
    }
}
