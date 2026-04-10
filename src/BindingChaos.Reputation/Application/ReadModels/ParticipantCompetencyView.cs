namespace BindingChaos.Reputation.Application.ReadModels;

/// <summary>A single competency entry in a participant's profile.</summary>
/// <param name="SkillId">The skill ID.</param>
/// <param name="DomainSlug">The domain's ASCII slug (e.g. <c>crafts</c>).</param>
/// <param name="Slug">The skill's ASCII slug within its domain (e.g. <c>carpentry</c>).</param>
/// <param name="SkillName">The localized skill name (best available locale).</param>
/// <param name="EndorsementCount">Total number of endorsements received.</param>
/// <param name="AverageGrade">Mean grade across all endorsements (1–5).</param>
public sealed record ParticipantCompetencyView(
    Guid SkillId,
    string DomainSlug,
    string Slug,
    string SkillName,
    int EndorsementCount,
    double AverageGrade);
