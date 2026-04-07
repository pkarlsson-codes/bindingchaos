using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.ResourceRequirements;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to withdraw a previously made pledge from a resource requirement.
/// </summary>
/// <param name="RequirementId">The resource requirement the pledge was made against.</param>
/// <param name="PledgeId">The pledge to withdraw.</param>
/// <param name="ActorId">The participant withdrawing the pledge.</param>
public sealed record WithdrawPledge(
    ResourceRequirementId RequirementId,
    PledgeId PledgeId,
    ParticipantId ActorId);

/// <summary>
/// Handles the <see cref="WithdrawPledge"/> command.
/// </summary>
public static class WithdrawPledgeHandler
{
    /// <summary>
    /// Withdraws a pledge. The domain enforces that only the original pledger may withdraw.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="requirementRepository">Repository used to load and persist the requirement.</param>
    /// <param name="unitOfWork">Unit of work for transaction boundaries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        WithdrawPledge command,
        IResourceRequirementRepository requirementRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var requirement = await requirementRepository
            .GetByIdOrThrowAsync(command.RequirementId, cancellationToken)
            .ConfigureAwait(false);

        requirement.WithdrawPledge(command.PledgeId, command.ActorId);

        requirementRepository.Stage(requirement);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
