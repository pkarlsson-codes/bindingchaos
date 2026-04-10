using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.ProjectInquiries.Events;
using BindingChaos.Stigmergy.Domain.Projects;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries;

/// <summary>
/// A formal inquiry raised by a Society member against a project.
/// Drives the project's outer-circle contestation status.
/// </summary>
public sealed class ProjectInquiry : AggregateRoot<ProjectInquiryId>
{
#pragma warning disable CS8618
    private ProjectInquiry()
    {
    }
#pragma warning restore CS8618

    /// <summary>Gets the project this inquiry is about.</summary>
    public ProjectId ProjectId { get; private set; }

    /// <summary>Gets the participant who raised the inquiry.</summary>
    public string RaisedByParticipantId { get; private set; }

    /// <summary>Gets the society that gives the raiser standing.</summary>
    public string RaisedBySocietyId { get; private set; }

    /// <summary>Gets the inquiry body text.</summary>
    public string Body { get; private set; }

    /// <summary>Gets the current lifecycle status.</summary>
    public ProjectInquiryStatus Status { get; private set; }

    /// <summary>Gets the user group's response, when provided.</summary>
    public string? Response { get; private set; }

    /// <summary>Gets the discourse thread ID linked to this inquiry, when created.</summary>
    public string? DiscourseThreadId { get; private set; }

    /// <summary>Gets the lapse window for this inquiry (from the society's social contract at raise time).</summary>
    public TimeSpan LapseWindow { get; private set; }

    /// <summary>Gets when this inquiry was last acted on (raised, responded, or updated).</summary>
    public DateTimeOffset LastActionAt { get; private set; }

    /// <summary>Raises a new inquiry against a project.</summary>
    /// <param name="projectId">The project being inquired about.</param>
    /// <param name="raisedBy">The participant raising the inquiry.</param>
    /// <param name="societyId">The society giving the raiser standing.</param>
    /// <param name="body">The inquiry body text.</param>
    /// <param name="lapseWindow">The lapse window from the society's social contract.</param>
    /// <returns>The new <see cref="ProjectInquiry"/> instance.</returns>
    public static ProjectInquiry Raise(
        ProjectId projectId,
        ParticipantId raisedBy,
        string societyId,
        string body,
        TimeSpan lapseWindow)
    {
        ArgumentNullException.ThrowIfNull(projectId);
        ArgumentNullException.ThrowIfNull(raisedBy);
        ArgumentException.ThrowIfNullOrWhiteSpace(societyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        var inquiry = new ProjectInquiry();
        var id = ProjectInquiryId.Generate();
        inquiry.ApplyChange(new ProjectInquiryRaised(
            id.Value,
            projectId.Value,
            raisedBy.Value,
            societyId,
            body,
            lapseWindow.Ticks));
        return inquiry;
    }

    /// <summary>Records the user group's response. Moves status to Responded.</summary>
    /// <param name="response">The response text.</param>
    public void Respond(string response)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(response);
        if (Status != ProjectInquiryStatus.Open)
        {
            throw new BusinessRuleViolationException("Only Open inquiries can be responded to.");
        }

        ApplyChange(new ProjectInquiryResponded(Id.Value, response));
    }

    /// <summary>Raiser accepts the response. Moves status to Resolved.</summary>
    /// <param name="actorId">The participant resolving the inquiry; must be the original raiser.</param>
    public void Resolve(ParticipantId actorId)
    {
        ArgumentNullException.ThrowIfNull(actorId);
        if (Status != ProjectInquiryStatus.Responded)
        {
            throw new BusinessRuleViolationException("Only Responded inquiries can be resolved.");
        }

        if (actorId.Value != RaisedByParticipantId)
        {
            throw new BusinessRuleViolationException("Only the raiser can resolve an inquiry.");
        }

        ApplyChange(new ProjectInquiryResolved(Id.Value, actorId.Value, ProjectId.Value));
    }

    /// <summary>Raiser updates the inquiry body. Resets status to Open.</summary>
    /// <param name="actorId">The participant updating the inquiry; must be the original raiser.</param>
    /// <param name="newBody">The new body text.</param>
    public void Update(ParticipantId actorId, string newBody)
    {
        ArgumentNullException.ThrowIfNull(actorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(newBody);
        if (actorId.Value != RaisedByParticipantId)
        {
            throw new BusinessRuleViolationException("Only the raiser can update an inquiry.");
        }

        if (Status != ProjectInquiryStatus.Open && Status != ProjectInquiryStatus.Responded)
        {
            throw new BusinessRuleViolationException("Only Open or Responded inquiries can be updated.");
        }

        ApplyChange(new ProjectInquiryUpdated(Id.Value, actorId.Value, newBody));
    }

    /// <summary>Auto-closes the inquiry if still open. No-op if already Resolved or Lapsed.</summary>
    public void Lapse()
    {
        if (Status != ProjectInquiryStatus.Open && Status != ProjectInquiryStatus.Responded)
        {
            return;
        }

        ApplyChange(new ProjectInquiryLapsed(Id.Value, ProjectId.Value));
    }

    /// <summary>Raiser reopens a lapsed inquiry.</summary>
    /// <param name="actorId">The participant reopening the inquiry; must be the original raiser.</param>
    /// <param name="updatedBody">Optional updated body text.</param>
    public void Reopen(ParticipantId actorId, string? updatedBody)
    {
        ArgumentNullException.ThrowIfNull(actorId);
        if (Status != ProjectInquiryStatus.Lapsed)
        {
            throw new BusinessRuleViolationException("Only Lapsed inquiries can be reopened.");
        }

        if (actorId.Value != RaisedByParticipantId)
        {
            throw new BusinessRuleViolationException("Only the raiser can reopen an inquiry.");
        }

        ApplyChange(new ProjectInquiryReopened(Id.Value, actorId.Value, updatedBody));
    }

    /// <summary>Links a discourse thread to this inquiry. Called once after thread creation.</summary>
    /// <param name="discourseThreadId">The ID of the discourse thread to link.</param>
    public void LinkDiscourseThread(string discourseThreadId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(discourseThreadId);
        if (DiscourseThreadId is not null)
        {
            throw new BusinessRuleViolationException("A discourse thread is already linked.");
        }

        ApplyChange(new ProjectInquiryDiscourseThreadLinked(Id.Value, discourseThreadId));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ProjectInquiryRaised e: Apply(e); break;
            case ProjectInquiryResponded e: Apply(e); break;
            case ProjectInquiryResolved e: Apply(e); break;
            case ProjectInquiryUpdated e: Apply(e); break;
            case ProjectInquiryLapsed e: Apply(e); break;
            case ProjectInquiryReopened e: Apply(e); break;
            case ProjectInquiryDiscourseThreadLinked e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(ProjectInquiryRaised e)
    {
        Id = ProjectInquiryId.Create(e.AggregateId);
        ProjectId = ProjectId.Create(e.ProjectId);
        RaisedByParticipantId = e.RaisedByParticipantId;
        RaisedBySocietyId = e.RaisedBySocietyId;
        Body = e.Body;
        Status = ProjectInquiryStatus.Open;
        LapseWindow = TimeSpan.FromTicks(e.LapseWindowTicks);
        LastActionAt = e.OccurredAt;
    }

    private void Apply(ProjectInquiryResponded e)
    {
        Response = e.Response;
        Status = ProjectInquiryStatus.Responded;
        LastActionAt = e.OccurredAt;
    }

    private void Apply(ProjectInquiryResolved e)
    {
        _ = e;
        Status = ProjectInquiryStatus.Resolved;
    }

    private void Apply(ProjectInquiryUpdated e)
    {
        Body = e.NewBody;
        Response = null;
        Status = ProjectInquiryStatus.Open;
        LastActionAt = e.OccurredAt;
    }

    private void Apply(ProjectInquiryLapsed e)
    {
        _ = e;
        Status = ProjectInquiryStatus.Lapsed;
    }

    private void Apply(ProjectInquiryReopened e)
    {
        if (e.UpdatedBody is not null)
        {
            Body = e.UpdatedBody;
        }

        Response = null;
        Status = ProjectInquiryStatus.Open;
        LastActionAt = e.OccurredAt;
    }

    private void Apply(ProjectInquiryDiscourseThreadLinked e)
    {
        DiscourseThreadId = e.DiscourseThreadId;
    }
}
