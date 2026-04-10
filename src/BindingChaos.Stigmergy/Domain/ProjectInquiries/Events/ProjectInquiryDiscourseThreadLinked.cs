using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;

/// <summary>Raised when a discourse thread is linked to a project inquiry.</summary>
/// <param name="AggregateId">The inquiry ID.</param>
/// <param name="DiscourseThreadId">The ID of the linked discourse thread.</param>
public sealed record ProjectInquiryDiscourseThreadLinked(
    string AggregateId,
    string DiscourseThreadId
) : DomainEvent(AggregateId);
