using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.DTOs;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class FormUserGroupHandlerTests
{
    private class TestBed
    {
        public Mock<IUserGroupRepository> UserGroupRepository { get; } = new();
        public Mock<ICommonsRepository> CommonsRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenCommonsNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.CommonsRepository
                .Setup(r => r.ExistsByIdAsync(It.IsAny<CommonsId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var command = CreateCommand(CommonsId.Generate());

            var act = async () => await FormUserGroupHandler.Handle(
                command, testBed.UserGroupRepository.Object, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenStagesUserGroupAndCommits()
        {
            var commonsId = CommonsId.Generate();
            testBed.CommonsRepository
                .Setup(r => r.ExistsByIdAsync(commonsId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var command = CreateCommand(commonsId);

            var result = await FormUserGroupHandler.Handle(
                command, testBed.UserGroupRepository.Object, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UserGroupRepository.Verify(r => r.Stage(It.IsAny<UserGroup>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            result.Should().NotBeNull();
        }

        private static FormUserGroup CreateCommand(CommonsId commonsId) =>
            new(commonsId,
                ParticipantId.Generate(),
                "Water Stewards",
                "Collective stewardship of water resources",
                new CharterDto(
                    new ContestationRulesDto(TimeSpan.FromDays(3), 0.5m),
                    new MembershipRulesDto(JoinPolicyDto.Open, null, null, null, true),
                    new ShunningRulesDto(0.6m)));
    }
}
