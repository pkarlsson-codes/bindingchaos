using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Concerns;
using BindingChaos.Stigmergy.Domain.Signals;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Raise concern command.
/// </summary>
/// <param name="ActorId">Id of the actor raising the concern.</param>
/// <param name="Name">Name of the raised concern.</param>
/// <param name="SignalIds">Ids of the signals surfacing the concern.</param>
public sealed record RaiseConcern(ParticipantId ActorId, string Name, IReadOnlyList<SignalId> SignalIds);

/// <summary>
/// A <see cref="RaiseConcern"/> command handler.
/// </summary>
public static class RaiseConcernHandler
{
    /// <summary>
    /// Handles an <see cref="RaiseConcern"/> command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="concernRepository">A concern repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Id of the raised concern.</returns>
    public static async Task<ConcernId> Handle(
        RaiseConcern command,
        IConcernRepository concernRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var concern = Concern.Raise(command.ActorId, command.Name, command.SignalIds);
        concernRepository.Stage(concern);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return concern.Id;
    }
}