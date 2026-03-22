using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class ContestAmendmentHandlerTests
{
    private class TestBed
    {
        public Mock<IDocumentSession> Session { get; } = new();
        public Mock<IMessageBus> MessageBus { get; } = new();
    }

    private static UserGroup CreateUserGroup()
    {
        var charter = new Charter(
            new ContentionRules(0.5m, TimeSpan.FromDays(3)),
            new MembershipRules(JoinPolicy.Open, true, null, null, null),
            new ShunningRules(0.6m));
        return UserGroup.Create(ParticipantId.Generate(), "Test Group", charter);
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenProjectNotFound_WhenHandled_ThenThrowsInvalidOperation()
        {
            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);
            var command = new ContestAmendment("project-abc", "amendment-xyz", "participant-123");

            var act = async () => await ContestAmendmentHandler.Handle(
                command, testBed.Session.Object, testBed.MessageBus.Object, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenContesterIsNotMember_WhenHandled_ThenThrowsBusinessRuleViolation()
        {
            var userGroup = CreateUserGroup();
            var project = Project.Create(userGroup.Id, "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());

            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.Session
                .Setup(s => s.LoadAsync<UserGroup>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var nonMember = ParticipantId.Generate();
            var command = new ContestAmendment(project.Id.Value, amendmentId.Value, nonMember.Value);

            var act = async () => await ContestAmendmentHandler.Handle(
                command, testBed.Session.Object, testBed.MessageBus.Object, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenAmendmentIsContested()
        {
            var userGroup = CreateUserGroup();
            var project = Project.Create(userGroup.Id, "Title", "Desc");
            var member = userGroup.Members.First().ParticipantId;
            var amendmentId = project.ProposeAmendment(member);

            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.Session
                .Setup(s => s.LoadAsync<UserGroup>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var command = new ContestAmendment(project.Id.Value, amendmentId.Value, member.Value);

            await ContestAmendmentHandler.Handle(
                command, testBed.Session.Object, testBed.MessageBus.Object, CancellationToken.None);

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

            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.Session
                .Setup(s => s.LoadAsync<UserGroup>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var command = new ContestAmendment(project.Id.Value, amendmentId.Value, member.Value);

            await ContestAmendmentHandler.Handle(
                command, testBed.Session.Object, testBed.MessageBus.Object, CancellationToken.None);

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

            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            testBed.Session
                .Setup(s => s.LoadAsync<UserGroup>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);

            var command = new ContestAmendment(project.Id.Value, amendmentId.Value, member.Value);

            await ContestAmendmentHandler.Handle(
                command, testBed.Session.Object, testBed.MessageBus.Object, CancellationToken.None);

            testBed.Session.Verify(s => s.Store(It.IsAny<Project>()), Times.Once);
            testBed.Session.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
