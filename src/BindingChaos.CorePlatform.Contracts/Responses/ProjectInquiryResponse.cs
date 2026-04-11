namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>Response model for a project inquiry.</summary>
/// <param name="Id">The inquiry identifier.</param>
/// <param name="ProjectId">The project this inquiry is about.</param>
/// <param name="RaisedByParticipantId">The participant who raised the inquiry.</param>
/// <param name="RaisedBySocietyId">The society that gives the raiser standing.</param>
/// <param name="Body">The inquiry body text.</param>
/// <param name="Status">The current lifecycle status.</param>
/// <param name="Response">The user group's response, when provided.</param>
/// <param name="DiscourseThreadId">The linked discourse thread identifier, when created.</param>
/// <param name="RaisedAt">When the inquiry was raised.</param>
/// <param name="LastUpdatedAt">When the inquiry was last updated.</param>
public sealed record ProjectInquiryResponse(
    string Id,
    string ProjectId,
    string RaisedByParticipantId,
    string RaisedBySocietyId,
    string Body,
    string Status,
    string? Response,
    string? DiscourseThreadId,
    DateTimeOffset RaisedAt,
    DateTimeOffset LastUpdatedAt);
