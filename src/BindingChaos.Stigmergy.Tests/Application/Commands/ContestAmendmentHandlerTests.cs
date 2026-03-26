using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Moq;
using Wolverine;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class ContestAmendmentHandlerTests
{
    private class TestBed
    {
        public Mock<IProjectRepository> ProjectRepository { get; } = new();
        public Mock<IUserGroupRepository> UserGroupRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IMessageBus> MessageBus { get; } = new();
    }

    private static UserGroup CreateUserGroup()
    {
        var charter = new Charter(
            new ContentionRules(0.5m, TimeSpan.FromDays(3)),
            new MembershipRules(JoinPolicy.Open, true, null, null, null),
            new ShunningRules(0.6m));
        return UserGroup.Form(
            ParticipantId.Generate(),
            CommonsId.Generate(),
            "Test Group",
            "Test Philosophy",
            charter);
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenProjectNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Project), ProjectId.Create("project-abc")));
            var command = new ContestAmendment(ProjectId.Create("project-abc"), AmendmentId.Create("amendment-xyz"), ParticipantId.Create("participant-123"));

            var act = async () => await ContestAmendmentHandler.Handle(
                command, testBed.ProjectRepository.Object, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, testBed.MessageBus.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenContesterIsNotMember_WhenHandled_ThenThrowsBusinessRuleViolation()
        {
            var userGroup = CreateUserGroup();
            var project = Project.Create(userGroup.Id, "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());

            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var nonMember = ParticipantId.Generate();
            var command = new ContestAmendment(project.Id, amendmentId, nonMember);

            var act = async () => await ContestAmendmentHandler.Handle(
                command, testBed.ProjectRepository.Object, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, testBed.MessageBus.Object, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenAmendmentIsContested()
        {
            var userGroup = CreateUserGroup();
            var project = Project.Create(userGroup.Id, "Title", "Desc");
            var member = userGroup.Members.First().ParticipantId;
            var amendmentId = project.ProposeAmendment(member);

            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var command = new ContestAmendment(project.Id, amendmentId, member);

            await ContestAmendmentHandler.Handle(
                command, testBed.ProjectRepository.Object, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, testBed.MessageBus.Object, CancellationToken.None);

            project.Amendments.Single(a => a.Id == amendmentId).Status
                .Should().Be(AmendmentStatus.Contested);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenPublishesAmendmentContentionStarted()
        {
            var userGroup = CreateUserGroup();
            var project = Project.Create(userGroup.Id, "Title", "Desc");
            var member = userGroup.Members.First().ParticipantId;
            var amendmentId = project.ProposeAmendment(member);

            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var command = new ContestAmendment(project.Id, amendmentId, member);

            await ContestAmendmentHandler.Handle(
                command, testBed.ProjectRepository.Object, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, testBed.MessageBus.Object, CancellationToken.None);

            testBed.MessageBus.Verify(
                b => b.PublishAsync(
                    It.Is<AmendmentContentionStarted>(m =>
                        m.AmendmentId == amendmentId.Value &&
                        m.ContesterId == member.Value &&
                        m.RejectionThreshold == 0.5m),
                    It.IsAny<DeliveryOptions?>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenSavesChanges()
        {
            var userGroup = CreateUserGroup();
            var project = Project.Create(userGroup.Id, "Title", "Desc");
            var member = userGroup.Members.First().ParticipantId;
            var amendmentId = project.ProposeAmendment(member);

            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var command = new ContestAmendment(project.Id, amendmentId, member);

            await ContestAmendmentHandler.Handle(
                command, testBed.ProjectRepository.Object, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, testBed.MessageBus.Object, CancellationToken.None);

            testBed.ProjectRepository.Verify(r => r.Stage(It.IsAny<Project>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
