using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to withdraw trust from one participant to another.
/// </summary>
/// <param name="TrusterId">The participant withdrawing trust.</param>
/// <param name="TrusteeId">The participant whose trust is being withdrawn.</param>
public sealed record WithdrawTrust(ParticipantId TrusterId, ParticipantId TrusteeId);

/// <summary>
/// Handles the <see cref="WithdrawTrust"/> command.
/// </summary>
public static class WithdrawTrustHandler
{
    /// <summary>
    /// Removes the trust relationship from <paramref name="command"/>.<see cref="WithdrawTrust.TrusterId"/>
    /// to <paramref name="command"/>.<see cref="WithdrawTrust.TrusteeId"/>.
    /// Idempotent — if the relationship does not exist, this is a no-op.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The trust relationship repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        WithdrawTrust command,
        ITrustRelationshipRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        await repository.WithdrawAsync(command.TrusterId, command.TrusteeId, cancellationToken).ConfigureAwait(false);
    }
}
