using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Societies.Domain.Societies;

namespace BindingChaos.Societies.Application.Commands;

/// <summary>Declares that a society is affected by a commons.</summary>
/// <param name="SocietyId">The society making the declaration.</param>
/// <param name="CommonsId">The ID of the commons this society is affected by.</param>
/// <param name="ActorId">The participant making the declaration.</param>
public sealed record DeclareSocietyAffectedByCommons(
    SocietyId SocietyId,
    string CommonsId,
    ParticipantId ActorId);

/// <summary>Handles <see cref="DeclareSocietyAffectedByCommons"/>.</summary>
public static class DeclareSocietyAffectedByCommonsHandler
{
    /// <summary>Handles the command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="societyRepository">Repository for loading and staging the society.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        DeclareSocietyAffectedByCommons command,
        ISocietyRepository societyRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var society = await societyRepository
            .GetByIdOrThrowAsync(command.SocietyId, cancellationToken)
            .ConfigureAwait(false);

        society.DeclareAffectedByCommons(command.CommonsId, command.ActorId);
        societyRepository.Stage(society);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
