using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class ResolveProjectInquiryHandlerTests
{
    private class TestBed
    {
        public Mock<IProjectInquiryRepository> InquiryRepository { get; } = new();

        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    [Fact]
    public async Task GivenRespondedInquiry_WhenHandledByRaiser_ThenInquiryIsResolved()
    {
        var bed = new TestBed();
        var raiser = ParticipantId.Generate();
        var inquiry = ProjectInquiry.Raise(ProjectId.Generate(), raiser, "s", "q", TimeSpan.FromDays(14));
        inquiry.Respond("done");
        bed.InquiryRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectInquiryId>(), default)).ReturnsAsync(inquiry);

        await ResolveProjectInquiryHandler.Handle(
            new ResolveProjectInquiry(inquiry.Id, raiser),
            bed.InquiryRepository.Object,
            bed.UnitOfWork.Object,
            default);

        inquiry.Status.Should().Be(ProjectInquiryStatus.Resolved);
    }

    [Fact]
    public async Task GivenRespondedInquiry_WhenHandledByNonRaiser_ThenThrowsBusinessRuleViolation()
    {
        var bed = new TestBed();
        var inquiry = ProjectInquiry.Raise(ProjectId.Generate(), ParticipantId.Generate(), "s", "q", TimeSpan.FromDays(14));
        inquiry.Respond("done");
        bed.InquiryRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectInquiryId>(), default)).ReturnsAsync(inquiry);

        var act = async () => await ResolveProjectInquiryHandler.Handle(
            new ResolveProjectInquiry(inquiry.Id, ParticipantId.Generate()),
            bed.InquiryRepository.Object,
            bed.UnitOfWork.Object,
            default);

        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }
}
