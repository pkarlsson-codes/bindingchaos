using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Ideation.Domain.Ideas.Events;

/// <summary>
/// Domain event raised when an idea is amended.
/// </summary>
/// <param name="AggregateId">The ID of the idea.</param>
/// <param name="AmendmentId">The amendment ID that was applied.</param>
/// <param name="NewVersionNumber">The new version number.</param>
/// <param name="NewTitle">The new title.</param>
/// <param name="NewBody">The new body content.</param>
public sealed record IdeaAmended(
    string AggregateId,
    string AmendmentId,
    int NewVersionNumber,
    string NewTitle,
    string NewBody
) : DomainEvent(AggregateId);