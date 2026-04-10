using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using FluentAssertions;

namespace BindingChaos.Stigmergy.Tests.Domain.ProjectInquiries;

public class ProjectInquiryTests
{
    private static ProjectInquiry BuildRaisedInquiry(out ParticipantId raiser)
    {
        raiser = ParticipantId.Generate();
        return ProjectInquiry.Raise(
            ProjectId.Generate(),
            raiser,
            "society-abc",
            "Is this safe for the river?",
            TimeSpan.FromDays(14));
    }

    [Fact]
    public void Raise_CreatesInquiryInOpenStatus()
    {
        var inquiry = BuildRaisedInquiry(out _);
        inquiry.Status.Should().Be(ProjectInquiryStatus.Open);
    }

    [Fact]
    public void Respond_MovesToRespondedStatus()
    {
        var inquiry = BuildRaisedInquiry(out _);
        inquiry.Respond("We have addressed this in section 3.");
        inquiry.Status.Should().Be(ProjectInquiryStatus.Responded);
    }

    [Fact]
    public void Resolve_WhenResponded_MovesToResolvedStatus()
    {
        var inquiry = BuildRaisedInquiry(out var raiser);
        inquiry.Respond("Addressed.");
        inquiry.Resolve(raiser);
        inquiry.Status.Should().Be(ProjectInquiryStatus.Resolved);
    }

    [Fact]
    public void Resolve_WhenNotResponded_ThrowsBusinessRuleViolation()
    {
        var inquiry = BuildRaisedInquiry(out var raiser);
        var act = () => inquiry.Resolve(raiser);
        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Resolve_ByNonRaiser_ThrowsBusinessRuleViolation()
    {
        var inquiry = BuildRaisedInquiry(out _);
        inquiry.Respond("Addressed.");
        var act = () => inquiry.Resolve(ParticipantId.Generate());
        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Update_ResetsToOpenStatus()
    {
        var inquiry = BuildRaisedInquiry(out var raiser);
        inquiry.Respond("Addressed.");
        inquiry.Update(raiser, "New concern about water quality.");
        inquiry.Status.Should().Be(ProjectInquiryStatus.Open);
        inquiry.Body.Should().Be("New concern about water quality.");
    }

    [Fact]
    public void Lapse_WhenOpen_MovesToLapsedStatus()
    {
        var inquiry = BuildRaisedInquiry(out _);
        inquiry.Lapse();
        inquiry.Status.Should().Be(ProjectInquiryStatus.Lapsed);
    }

    [Fact]
    public void Lapse_WhenAlreadyResolved_DoesNothing()
    {
        var inquiry = BuildRaisedInquiry(out var raiser);
        inquiry.Respond("Done.");
        inquiry.Resolve(raiser);
        inquiry.Lapse();
        inquiry.Status.Should().Be(ProjectInquiryStatus.Resolved);
    }

    [Fact]
    public void Reopen_WhenLapsed_MovesToOpenStatus()
    {
        var inquiry = BuildRaisedInquiry(out var raiser);
        inquiry.Lapse();
        inquiry.Reopen(raiser, null);
        inquiry.Status.Should().Be(ProjectInquiryStatus.Open);
    }
}
