using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to extend trust from one participant to another.
/// </summary>
/// <param name="TrusterId">The participant extending trust.</param>
/// <param name="TrusteeId">The participant being trusted.</param>
public sealed record ExtendTrust(ParticipantId TrusterId, ParticipantId TrusteeId);

/// <summary>
/// Handles the <see cref="ExtendTrust"/> command.
/// </summary>
public static class ExtendTrustHandler
{
    /// <summary>
    /// Creates a trust relationship from <paramref name="command"/>.<see cref="ExtendTrust.TrusterId"/>
    /// to <paramref name="command"/>.<see cref="ExtendTrust.TrusteeId"/> and persists it.
    /// Idempotent — if the relationship already exists, this is a no-op.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The trust relationship repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ExtendTrust command,
        ITrustRelationshipRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var relationship = TrustRelationship.Create(command.TrusterId, command.TrusteeId);
        await repository.TrustAsync(relationship, cancellationToken).ConfigureAwait(false);
    }
}
