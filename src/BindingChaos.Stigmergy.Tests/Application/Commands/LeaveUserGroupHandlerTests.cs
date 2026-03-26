using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class LeaveUserGroupHandlerTests
{
    private class TestBed
    {
        public Mock<IUserGroupRepository> UserGroupRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    private static UserGroup CreateOpenGroup()
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
        public async Task GivenGroupNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(UserGroup), UserGroupId.Generate()));
            var command = new LeaveUserGroup(UserGroupId.Generate(), ParticipantId.Generate());

            var act = async () => await LeaveUserGroupHandler.Handle(
                command, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenValidMember_WhenHandled_ThenMemberIsRemoved()
        {
            var group = CreateOpenGroup();
            var member = ParticipantId.Generate();
            group.ApplyToJoin(member);

            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(group);
            var command = new LeaveUserGroup(group.Id, member);

            await LeaveUserGroupHandler.Handle(command, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            group.Members.Should().NotContain(m => m.ParticipantId == member);
        }

        [Fact]
        public async Task GivenValidMember_WhenHandled_ThenSavesChanges()
        {
            var group = CreateOpenGroup();
            var member = ParticipantId.Generate();
            group.ApplyToJoin(member);

            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(group);
            var command = new LeaveUserGroup(group.Id, member);

            await LeaveUserGroupHandler.Handle(command, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UserGroupRepository.Verify(r => r.Stage(It.IsAny<UserGroup>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
