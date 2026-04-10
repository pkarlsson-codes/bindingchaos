namespace BindingChaos.Reputation.Domain.SkillEndorsements;

/// <summary>
/// Represents the grade assigned when endorsing a participant's competence in a skill.
/// </summary>
public enum EndorsementGrade
{
    /// <summary>The endorser has observed the participant work with this skill.</summary>
    Familiar = 1,

    /// <summary>The participant can perform this skill reliably.</summary>
    Competent = 2,

    /// <summary>The participant performs this skill well.</summary>
    Proficient = 3,

    /// <summary>The participant is a go-to resource for this skill.</summary>
    Expert = 4,

    /// <summary>The participant could teach this skill at a high level.</summary>
    Master = 5,
}
