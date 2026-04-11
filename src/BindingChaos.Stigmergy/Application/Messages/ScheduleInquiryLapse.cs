namespace BindingChaos.Stigmergy.Application.Messages;

/// <summary>Scheduled message that triggers the lapse check for an inquiry.</summary>
/// <param name="InquiryId">The inquiry to evaluate for lapsing.</param>
public sealed record ScheduleInquiryLapse(string InquiryId);
