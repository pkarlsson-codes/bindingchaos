using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Represents a command to withdraw support for an amendment.
/// </summary>
/// <param name="AmendmentId">The unique identifier of the amendment to withdraw support from.</param>
/// <param name="ParticipantId">The unique identifier of the participant withdrawing support.</param>
public sealed record WithdrawSupport(
    AmendmentId AmendmentId,
    ParticipantId ParticipantId);

/// <summary>Handles the <see cref="WithdrawSupport"/> command to withdraw support for an amendment.</summary>
public static class WithdrawSupportHandler
{
    /// <summary>Withdraws a participant's support from an amendment and returns the updated vote counts.</summary>
    /// <param name="request">The withdraw support command.</param>
    /// <param name="amendmentRepository">Repository for loading and staging the amendment.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts.</returns>
    public static async Task<AmendmentSupportCounts> Handle(
        WithdrawSupport request,
        IAmendmentRepository amendmentRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var amendment = await amendmentRepository.GetByIdAsync(request.AmendmentId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Amendment {request.AmendmentId} not found");

        amendment.WithdrawSupport(request.ParticipantId);
        amendmentRepository.Stage(amendment);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return new AmendmentSupportCounts(amendment.Supporters.Count, amendment.Opponents.Count);
    }
}
