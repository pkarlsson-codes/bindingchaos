namespace BindingChaos.Stigmergy.Application.Messages;

/// <summary>
/// Scheduled message sent by the amendment contention saga to itself
/// when the resolution window expires. Wolverine routes this back to the saga via
/// the <c>AmendmentContentionSagaId</c> property.
/// </summary>
/// <param name="AmendmentContentionSagaId">The saga correlation ID (equals the AmendmentId).</param>
/// <param name="ProjectId">The identifier of the project containing the amendment.</param>
public sealed record ResolveAmendmentContention(
    string AmendmentContentionSagaId,
    string ProjectId);
