using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.Reputation.Domain.SkillEndorsements;

/// <summary>
/// Represents a directed, graded endorsement from one participant to another for a specific skill.
/// Identity is the composite (EndorserId, EndorseeId, SkillId).
/// </summary>
public sealed class SkillEndorsement : IEquatable<SkillEndorsement>
{
    private SkillEndorsement(
        ParticipantId endorserId,
        ParticipantId endorseeId,
        Guid skillId,
        EndorsementGrade grade,
        DateTimeOffset createdAt)
    {
        EndorserId = endorserId;
        EndorseeId = endorseeId;
        SkillId = skillId;
        Grade = grade;
        CreatedAt = createdAt;
    }

    /// <summary>Gets the ID of the participant giving the endorsement.</summary>
    public ParticipantId EndorserId { get; }

    /// <summary>Gets the ID of the participant being endorsed.</summary>
    public ParticipantId EndorseeId { get; }

    /// <summary>Gets the ID of the skill being endorsed.</summary>
    public Guid SkillId { get; }

    /// <summary>Gets the grade assigned to this endorsement.</summary>
    public EndorsementGrade Grade { get; private set; }

    /// <summary>Gets the UTC timestamp when this endorsement was created.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Gets the UTC timestamp when this endorsement's grade was last revised, if ever.</summary>
    public DateTimeOffset? UpdatedAt { get; private set; }

    /// <summary>Determines equality by composite (EndorserId, EndorseeId, SkillId) identity.</summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><see langword="true"/> if equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(SkillEndorsement? left, SkillEndorsement? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>Determines inequality by composite (EndorserId, EndorseeId, SkillId) identity.</summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><see langword="true"/> if not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(SkillEndorsement? left, SkillEndorsement? right)
        => !(left == right);

    /// <summary>
    /// Creates a new endorsement from <paramref name="endorserId"/> to <paramref name="endorseeId"/>
    /// for <paramref name="skillId"/> at <paramref name="grade"/>.
    /// </summary>
    /// <param name="endorserId">The participant giving the endorsement.</param>
    /// <param name="endorseeId">The participant being endorsed.</param>
    /// <param name="skillId">The skill being endorsed.</param>
    /// <param name="grade">The grade assigned.</param>
    /// <returns>The new endorsement.</returns>
    /// <exception cref="InvariantViolationException">Thrown when endorserId equals endorseeId.</exception>
    public static SkillEndorsement Create(
        ParticipantId endorserId, ParticipantId endorseeId, Guid skillId, EndorsementGrade grade)
    {
        if (endorserId == endorseeId)
        {
            throw new InvariantViolationException("A participant cannot endorse themselves.");
        }

        return new SkillEndorsement(endorserId, endorseeId, skillId, grade, TimeProviderContext.Current.UtcNow);
    }

    /// <summary>Updates the grade of an existing endorsement.</summary>
    /// <param name="newGrade">The new grade to assign.</param>
    public void Revise(EndorsementGrade newGrade)
    {
        Grade = newGrade;
        UpdatedAt = TimeProviderContext.Current.UtcNow;
    }

    /// <inheritdoc />
    public bool Equals(SkillEndorsement? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EndorserId == other.EndorserId && EndorseeId == other.EndorseeId && SkillId == other.SkillId;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as SkillEndorsement);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(EndorserId, EndorseeId, SkillId);
}
