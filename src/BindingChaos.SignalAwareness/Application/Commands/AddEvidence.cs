using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Domain.Evidence;
using BindingChaos.SignalAwareness.Domain.Signals;

namespace BindingChaos.SignalAwareness.Application.Commands;

/// <summary>
/// Represents a request to associate one or more evidence documents with a specific signal.
/// </summary>
/// <remarks>This record is immutable. All parameters must be provided when creating an instance.</remarks>
/// <param name="SignalId">The unique identifier of the signal to which the evidence is being added.</param>
/// <param name="DocumentIds">An array of document identifiers representing the evidence to associate with the signal. Cannot be null or contain
/// null elements.</param>
/// <param name="Description">A description providing context or details about the evidence being added.</param>
/// <param name="AddedBy">The identifier of the participant who is adding the evidence.</param>
public sealed record AddEvidence(SignalId SignalId, string[] DocumentIds, string Description, ParticipantId AddedBy);

/// <summary>
/// Provides functionality to add evidence to the system based on the specified command and repositories.
/// </summary>
public static class AddEvidenceHandler
{
    /// <summary>
    /// Handles an <see cref="AddEvidence"/> command.
    /// </summary>
    /// <param name="request">The command to handle.</param>
    /// <param name="signalRepository">A repository for <see cref="Signal"/> aggregates.</param>
    /// <param name="evidenceRepository">A repository for <see cref="Evidence"/> aggregates.</param>
    /// <param name="unitOfWork">A <see cref="IUnitOfWork"/>.</param>
    /// <param name="cancellationToken">A token used to cancel the action.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        AddEvidence request,
        ISignalRepository signalRepository,
        IEvidenceRepository evidenceRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var signalExists = await signalRepository.ExistsByIdAsync(request.SignalId, cancellationToken).ConfigureAwait(false);
        if(!signalExists)
        {
            throw new AggregateNotFoundException($"Signal with ID {request.SignalId.Value} not found");
        }

        var evidence = Evidence.Add(request.SignalId, request.DocumentIds, request.Description, request.AddedBy);
        evidenceRepository.Stage(evidence);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}