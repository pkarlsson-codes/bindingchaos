using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;

/// <summary>Raised when the raiser updates the inquiry body (resetting it to Open).</summary>
/// <param name="AggregateId">The inquiry ID.</param>
/// <param name="UpdatedByParticipantId">The participant who updated the inquiry.</param>
/// <param name="NewBody">The new body text.</param>
public sealed record ProjectInquiryUpdated(
    string AggregateId,
    string UpdatedByParticipantId,
    string NewBody
) : DomainEvent(AggregateId);
