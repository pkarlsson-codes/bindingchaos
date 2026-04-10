using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;

/// <summary>Raised when an inquiry auto-closes due to inaction within the lapse window.</summary>
/// <param name="AggregateId">The inquiry ID.</param>
/// <param name="ProjectId">The project this inquiry belongs to (used for contestation status projection).</param>
public sealed record ProjectInquiryLapsed(
    string AggregateId,
    string ProjectId
) : DomainEvent(AggregateId);
