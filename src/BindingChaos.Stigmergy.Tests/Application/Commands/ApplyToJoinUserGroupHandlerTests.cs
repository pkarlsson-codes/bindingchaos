using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class ApplyToJoinUserGroupHandlerTests
{
    private class TestBed
    {
        public Mock<IUserGroupRepository> UserGroupRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
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
            var command = new ApplyToJoinUserGroup(UserGroupId.Generate(), ParticipantId.Generate());

            var act = async () => await ApplyToJoinUserGroupHandler.Handle(
                command, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenOpenGroup_WhenHandled_ThenStagesGroupAndCommits()
        {
            var group = CreateOpenGroup();
            testBed.UserGroupRepository
                .Setup(r => r.GetByIdOrThrowAsync(group.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(group);
            var command = new ApplyToJoinUserGroup(group.Id, ParticipantId.Generate());

            await ApplyToJoinUserGroupHandler.Handle(
                command, testBed.UserGroupRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UserGroupRepository.Verify(r => r.Stage(It.IsAny<UserGroup>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static UserGroup CreateOpenGroup()
        {
            var charter = new Charter(
                new ContentionRules(0.5m, TimeSpan.FromDays(3)),
                new MembershipRules(JoinPolicy.Open, true, null, null, null),
                new ShunningRules(0.6m));
            return UserGroup.Form(ParticipantId.Generate(), CommonsId.Generate(), "Test Group", "Test Philosophy", charter);
        }
    }
}
