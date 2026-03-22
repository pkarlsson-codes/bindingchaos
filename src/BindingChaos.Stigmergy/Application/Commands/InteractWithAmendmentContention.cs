namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command for a user group member to record their vote on a contested amendment.
/// Agree = supports the contention (wants amendment rejected).
/// Disagree = opposes the contention (wants amendment kept).
/// Votes can be changed during the resolution window.
/// Wolverine routes this to the saga via the <c>AmendmentContentionSagaId</c> property.
/// </summary>
/// <param name="AmendmentContentionSagaId">The saga correlation ID (equals the AmendmentId).</param>
/// <param name="ParticipantId">The participant recording the vote.</param>
/// <param name="AgreesWithContention">True if the participant agrees the amendment should be rejected.</param>
public sealed record InteractWithAmendmentContention(
    string AmendmentContentionSagaId,
    string ParticipantId,
    bool AgreesWithContention);
