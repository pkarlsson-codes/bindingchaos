using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.Reputation.Domain.TrustRelationships;

/// <summary>
/// Represents a directed trust relationship from one participant to another.
/// Identity is the composite (TrusterId, TrusteeId).
/// </summary>
public sealed class TrustRelationship : IEquatable<TrustRelationship>
{
    private TrustRelationship(ParticipantId trusterId, ParticipantId trusteeId, DateTimeOffset createdAt)
    {
        TrusterId = trusterId;
        TrusteeId = trusteeId;
        CreatedAt = createdAt;
    }

    /// <summary>Gets the ID of the participant who is extending trust.</summary>
    public ParticipantId TrusterId { get; }

    /// <summary>Gets the ID of the participant being trusted.</summary>
    public ParticipantId TrusteeId { get; }

    /// <summary>Gets the UTC timestamp when this relationship was created.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Determines equality by composite (TrusterId, TrusteeId) identity.</summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><see langword="true"/> if equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(TrustRelationship? left, TrustRelationship? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>Determines inequality by composite (TrusterId, TrusteeId) identity.</summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><see langword="true"/> if not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(TrustRelationship? left, TrustRelationship? right)
        => !(left == right);

    /// <summary>
    /// Creates a new trust relationship from <paramref name="trusterId"/> to <paramref name="trusteeId"/>.
    /// </summary>
    /// <param name="trusterId">The participant extending trust.</param>
    /// <param name="trusteeId">The participant being trusted.</param>
    /// <returns>The new trust relationship.</returns>
    /// <exception cref="InvariantViolationException">Thrown when trusterId equals trusteeId (value equality).</exception>
    public static TrustRelationship Create(ParticipantId trusterId, ParticipantId trusteeId)
    {
        if (trusterId == trusteeId)
        {
            throw new InvariantViolationException("A participant cannot trust themselves.");
        }

        return new TrustRelationship(trusterId, trusteeId, TimeProviderContext.Current.UtcNow);
    }

    /// <inheritdoc />
    public bool Equals(TrustRelationship? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return TrusterId == other.TrusterId && TrusteeId == other.TrusteeId;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as TrustRelationship);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(TrusterId, TrusteeId);
}
