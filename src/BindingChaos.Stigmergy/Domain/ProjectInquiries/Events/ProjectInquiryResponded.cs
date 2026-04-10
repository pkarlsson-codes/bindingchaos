using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;

/// <summary>Raised when the user group responds to a project inquiry.</summary>
/// <param name="AggregateId">The inquiry ID.</param>
/// <param name="Response">The response text.</param>
public sealed record ProjectInquiryResponded(
    string AggregateId,
    string Response
) : DomainEvent(AggregateId);
