using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Represents a command to support an amendment.
/// </summary>
/// <param name="AmendmentId">The unique identifier of the amendment to support.</param>
/// <param name="ParticipantId">The unique identifier of the participant supporting the amendment.</param>
/// <param name="Reason">The reason provided by the participant for supporting the amendment.</param>
public sealed record SupportAmendment(
    AmendmentId AmendmentId,
    ParticipantId ParticipantId,
    string Reason);

/// <summary>Handles the <see cref="SupportAmendment"/> command to add support for an amendment.</summary>
public static class SupportAmendmentHandler
{
    /// <summary>Adds a participant's support to an amendment and returns the updated vote counts.</summary>
    /// <param name="request">The support amendment command.</param>
    /// <param name="amendmentRepository">Repository for loading and staging the amendment.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts.</returns>
    public static async Task<AmendmentSupportCounts> Handle(
        SupportAmendment request,
        IAmendmentRepository amendmentRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var amendment = await amendmentRepository.GetByIdOrThrowAsync(request.AmendmentId, cancellationToken).ConfigureAwait(false);

        if (amendment.Opponents.Any(o => o.ParticipantId == request.ParticipantId))
        {
            throw new BusinessRuleViolationException("Participant must withdraw opposition before supporting this amendment.");
        }

        var supporter = new Supporter(request.ParticipantId, request.Reason);
        amendment.AddSupport(supporter);
        amendmentRepository.Stage(amendment);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return new AmendmentSupportCounts(amendment.Supporters.Count, amendment.Opponents.Count);
    }
}
