using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;

/// <summary>Raised when a new project inquiry is raised by an affected society member.</summary>
/// <param name="AggregateId">The inquiry ID.</param>
/// <param name="ProjectId">The project being inquired about.</param>
/// <param name="RaisedByParticipantId">The participant who raised the inquiry.</param>
/// <param name="RaisedBySocietyId">The society giving the raiser standing.</param>
/// <param name="Body">The inquiry body text.</param>
/// <param name="LapseWindowTicks">The lapse window in ticks.</param>
public sealed record ProjectInquiryRaised(
    string AggregateId,
    string ProjectId,
    string RaisedByParticipantId,
    string RaisedBySocietyId,
    string Body,
    long LapseWindowTicks
) : DomainEvent(AggregateId);
