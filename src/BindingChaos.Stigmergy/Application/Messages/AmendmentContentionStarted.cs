namespace BindingChaos.Stigmergy.Application.Messages;

/// <summary>
/// Message published to Wolverine when an amendment is contested, starting the
/// amendment contention saga resolution process.
/// </summary>
/// <param name="AmendmentId">The identifier of the contested amendment (used as saga correlation ID).</param>
/// <param name="ProjectId">The identifier of the project that owns the amendment.</param>
/// <param name="UserGroupId">The identifier of the user group that owns the project.</param>
/// <param name="RejectionThreshold">Snapshot of the user group's rejection threshold at contention time.</param>
/// <param name="ResolutionWindow">Snapshot of the user group's resolution window at contention time.</param>
/// <param name="ContesterId">The participant who contested the amendment (recorded as the first agree vote).</param>
public sealed record AmendmentContentionStarted(
    string AmendmentId,
    string ProjectId,
    string UserGroupId,
    decimal RejectionThreshold,
    TimeSpan ResolutionWindow,
    string ContesterId);
