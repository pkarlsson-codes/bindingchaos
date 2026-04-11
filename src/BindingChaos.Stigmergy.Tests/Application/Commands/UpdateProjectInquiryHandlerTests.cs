using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using FluentAssertions;
using Moq;
using Wolverine;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class UpdateProjectInquiryHandlerTests
{
    private class TestBed
    {
        public Mock<IProjectInquiryRepository> InquiryRepository { get; } = new();

        public Mock<IMessageContext> MessageContext { get; } = new();

        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    [Fact]
    public async Task GivenOpenInquiry_WhenHandledByRaiser_ThenBodyUpdatedAndStatusOpen()
    {
        var bed = new TestBed();
        var raiser = ParticipantId.Generate();
        var inquiry = ProjectInquiry.Raise(ProjectId.Generate(), raiser, "s", "original body", TimeSpan.FromDays(14));
        bed.InquiryRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectInquiryId>(), default)).ReturnsAsync(inquiry);

        await UpdateProjectInquiryHandler.Handle(
            new UpdateProjectInquiry(inquiry.Id, raiser, "updated body"),
            bed.InquiryRepository.Object,
            bed.MessageContext.Object,
            bed.UnitOfWork.Object,
            default);

        inquiry.Body.Should().Be("updated body");
        inquiry.Status.Should().Be(ProjectInquiryStatus.Open);
    }
}
