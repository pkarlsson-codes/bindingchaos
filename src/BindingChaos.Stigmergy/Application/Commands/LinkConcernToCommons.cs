using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Concerns;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Marten;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Links a concern to a commons, recording that the commons is organised around that concern.</summary>
/// <param name="CommonsId">The ID of the commons to link the concern to.</param>
/// <param name="ConcernId">The ID of the concern to link.</param>
/// <param name="ActorId">The ID of the participant performing the action.</param>
public sealed record LinkConcernToCommons(CommonsId CommonsId, ConcernId ConcernId, ParticipantId ActorId);

/// <summary>Handles the <see cref="LinkConcernToCommons"/> command.</summary>
public static class LinkConcernToCommonsHandler
{
    /// <summary>Handles the <see cref="LinkConcernToCommons"/> command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="commonsRepository">The repository for commons aggregate persistence.</param>
    /// <param name="concernRepository">The repository to check for the existence of the specified concern.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        LinkConcernToCommons command,
        ICommonsRepository commonsRepository,
        IConcernRepository concernRepository,
        IQuerySession querySession,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!await commonsRepository.ExistsByIdAsync(command.CommonsId, cancellationToken).ConfigureAwait(false))
        {
            throw new AggregateNotFoundException(typeof(Commons), command.CommonsId);
        }

        if (!await concernRepository.ExistsByIdAsync(command.ConcernId, cancellationToken).ConfigureAwait(false))
        {
            throw new AggregateNotFoundException(typeof(Concern), command.ConcernId);
        }

        var userGroups = await querySession.Query<UserGroupListItemView>()
            .Where(v => v.CommonsId == command.CommonsId.Value)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var isMember = userGroups.Any(g => g.MemberParticipantIds.Contains(command.ActorId.Value));

        if (!isMember)
        {
            throw new BusinessRuleViolationException("Actor is not a member of any user group governing this commons.");
        }

        var commons = await commonsRepository.GetByIdOrThrowAsync(command.CommonsId, cancellationToken).ConfigureAwait(false);
        commons.LinkConcern(command.ConcernId, command.ActorId);
        commonsRepository.Stage(commons);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
