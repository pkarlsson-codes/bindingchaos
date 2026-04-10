using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;

/// <summary>Raised when the raiser accepts the response and closes the inquiry.</summary>
/// <param name="AggregateId">The inquiry ID.</param>
/// <param name="ResolvedByParticipantId">The participant who resolved the inquiry.</param>
/// <param name="ProjectId">The project this inquiry belongs to (used for contestation status projection).</param>
public sealed record ProjectInquiryResolved(
    string AggregateId,
    string ResolvedByParticipantId,
    string ProjectId
) : DomainEvent(AggregateId);
