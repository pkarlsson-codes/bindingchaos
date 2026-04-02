using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Societies.Domain.SocialContracts;
using BindingChaos.Societies.Domain.Societies;

namespace BindingChaos.Societies.Application.Commands;

/// <summary>
/// Command to join a society.
/// </summary>
/// <param name="SocietyId">The society to join.</param>
/// <param name="ParticipantId">The participant joining the society.</param>
/// <param name="SocialContractId">The social contract the participant is agreeing to.</param>
/// <param name="InviteToken">The invite token used to join, if any. Stored for attribution.</param>
public sealed record JoinSociety(
    SocietyId SocietyId,
    ParticipantId ParticipantId,
    SocialContractId SocialContractId,
    string? InviteToken = null);

/// <summary>
/// Handles the <see cref="JoinSociety"/> command.
/// </summary>
public static class JoinSocietyHandler
{
    /// <summary>
    /// Handles a <see cref="JoinSociety"/> command, adding a participant membership to the society.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="societyRepository">Repository for loading and staging the society.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The identifier of the newly created membership.</returns>
    public static async Task<MembershipId> Handle(
        JoinSociety command,
        ISocietyRepository societyRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var society = await societyRepository
            .GetByIdOrThrowAsync(command.SocietyId, cancellationToken)
            .ConfigureAwait(false);

        var membershipId = society.Join(command.ParticipantId, command.SocialContractId, command.InviteToken);

        societyRepository.Stage(society);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return membershipId;
    }
}
