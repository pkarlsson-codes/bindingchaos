namespace BindingChaos.Reputation.Application.ReadModels;

/// <summary>A ranked expert entry for a skill.</summary>
/// <param name="ParticipantId">The expert's participant ID.</param>
/// <param name="EndorsementCount">Total number of endorsements received for this skill.</param>
/// <param name="AverageGrade">Mean grade across all endorsements (1–5).</param>
/// <param name="TotalScore">Sum of all grades; used for ranking.</param>
public sealed record SkillExpertView(string ParticipantId, int EndorsementCount, double AverageGrade, int TotalScore);
