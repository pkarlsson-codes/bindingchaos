using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Moq;
using Wolverine;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class RespondToProjectInquiryHandlerTests
{
    private class TestBed
    {
        public Mock<IProjectInquiryRepository> InquiryRepository { get; } = new();

        public Mock<IProjectRepository> ProjectRepository { get; } = new();

        public Mock<IUserGroupRepository> UserGroupRepository { get; } = new();

        public Mock<IMessageContext> MessageContext { get; } = new();

        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    private static (ProjectInquiry inquiry, UserGroup userGroup, Project project) BuildValidState()
    {
        var charter = new Charter(
            new ContentionRules(0.5m, TimeSpan.FromDays(3)),
            new MembershipRules(JoinPolicy.Open, true, null, null, null),
            new ShunningRules(0.6m));
        var userGroup = UserGroup.Form(ParticipantId.Generate(), CommonsId.Generate(), "G", "P", charter);
        var project = Project.Create(userGroup.Id, "Title", "Desc");
        var inquiry = ProjectInquiry.Raise(project.Id, ParticipantId.Generate(), "society-1", "A question", TimeSpan.FromDays(14));
        return (inquiry, userGroup, project);
    }

    [Fact]
    public async Task GivenUserGroupMember_WhenHandled_ThenInquiryIsResponded()
    {
        var bed = new TestBed();
        var (inquiry, userGroup, project) = BuildValidState();
        var member = userGroup.Members.First().ParticipantId;

        bed.InquiryRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectInquiryId>(), default)).ReturnsAsync(inquiry);
        bed.ProjectRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), default)).ReturnsAsync(project);
        bed.UserGroupRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), default)).ReturnsAsync(userGroup);

        var command = new RespondToProjectInquiry(inquiry.Id, project.Id, member, "We addressed it.");
        await RespondToProjectInquiryHandler.Handle(
            command,
            bed.InquiryRepository.Object,
            bed.ProjectRepository.Object,
            bed.UserGroupRepository.Object,
            bed.MessageContext.Object,
            bed.UnitOfWork.Object,
            default);

        inquiry.Status.Should().Be(ProjectInquiryStatus.Responded);
        inquiry.Response.Should().Be("We addressed it.");
    }

    [Fact]
    public async Task GivenNonMember_WhenHandled_ThenThrowsBusinessRuleViolation()
    {
        var bed = new TestBed();
        var (inquiry, userGroup, project) = BuildValidState();

        bed.InquiryRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectInquiryId>(), default)).ReturnsAsync(inquiry);
        bed.ProjectRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), default)).ReturnsAsync(project);
        bed.UserGroupRepository.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), default)).ReturnsAsync(userGroup);

        var command = new RespondToProjectInquiry(inquiry.Id, project.Id, ParticipantId.Generate(), "Response.");
        var act = async () => await RespondToProjectInquiryHandler.Handle(
            command,
            bed.InquiryRepository.Object,
            bed.ProjectRepository.Object,
            bed.UserGroupRepository.Object,
            bed.MessageContext.Object,
            bed.UnitOfWork.Object,
            default);

        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }
}
