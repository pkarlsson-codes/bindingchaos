using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;

/// <summary>Raised when the raiser reopens a lapsed inquiry.</summary>
/// <param name="AggregateId">The inquiry ID.</param>
/// <param name="ReopenedByParticipantId">The participant who reopened the inquiry.</param>
/// <param name="UpdatedBody">Optional updated body text.</param>
public sealed record ProjectInquiryReopened(
    string AggregateId,
    string ReopenedByParticipantId,
    string? UpdatedBody
) : DomainEvent(AggregateId);
