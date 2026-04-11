using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class LapseProjectInquiryHandlerTests
{
    private class TestBed
    {
        public Mock<IProjectInquiryRepository> InquiryRepository { get; } = new();

        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    [Fact]
    public async Task GivenOpenInquiry_WhenHandled_ThenInquiryIsLapsed()
    {
        var bed = new TestBed();
        var inquiry = ProjectInquiry.Raise(ProjectId.Generate(), ParticipantId.Generate(), "s", "q", TimeSpan.FromDays(14));
        bed.InquiryRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectInquiryId>(), default)).ReturnsAsync(inquiry);

        await LapseProjectInquiryHandler.Handle(
            new ScheduleInquiryLapse(inquiry.Id.Value),
            bed.InquiryRepository.Object,
            bed.UnitOfWork.Object,
            default);

        inquiry.Status.Should().Be(ProjectInquiryStatus.Lapsed);
        bed.UnitOfWork.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task GivenAlreadyResolvedInquiry_WhenHandled_ThenNoChangeAndNoCommit()
    {
        var bed = new TestBed();
        var raiser = ParticipantId.Generate();
        var inquiry = ProjectInquiry.Raise(ProjectId.Generate(), raiser, "s", "q", TimeSpan.FromDays(14));
        inquiry.Respond("done");
        inquiry.Resolve(raiser);
        bed.InquiryRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectInquiryId>(), default)).ReturnsAsync(inquiry);

        await LapseProjectInquiryHandler.Handle(
            new ScheduleInquiryLapse(inquiry.Id.Value),
            bed.InquiryRepository.Object,
            bed.UnitOfWork.Object,
            default);

        inquiry.Status.Should().Be(ProjectInquiryStatus.Resolved);
        bed.UnitOfWork.Verify(u => u.CommitAsync(default), Times.Never);
    }
}
