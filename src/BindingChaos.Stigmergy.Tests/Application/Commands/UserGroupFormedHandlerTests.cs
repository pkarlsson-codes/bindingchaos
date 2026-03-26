using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.EventHandlers;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using BindingChaos.Stigmergy.Domain.UserGroups;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class UserGroupFormedHandlerTests
{
    private class TestBed
    {
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
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<CommonsId>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Commons), CommonsId.Generate()));
            UserGroupFormed message = CreateUserGroupFormedEvent();

            var act = async () => await UserGroupFormedHandler.Handle(
                message, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenProposedCommons_WhenHandled_ThenSavesChanges()
        {
            var commons = Commons.Propose("Water", "Governing water", ParticipantId.Generate());
            commons.UncommittedEvents.MarkAsCommitted();
            List<IDomainEvent> events = [];
            testBed.CommonsRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<CommonsId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commons);
            testBed.CommonsRepository
                .Setup(r => r.Stage(It.IsAny<Commons>()))
                .Callback<Commons>(c => events.AddRange(c.UncommittedEvents));
            var message = CreateUserGroupFormedEvent();

            await UserGroupFormedHandler.Handle(message, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.CommonsRepository.Verify(r => r.Stage(It.IsAny<Commons>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            events.Should().ContainSingle().Which.Should().BeOfType<CommonsActivated>();
        }

        private static UserGroupFormed CreateUserGroupFormedEvent()
        {
            return new UserGroupFormed(
                UserGroupId.Generate().Value,
                CommonsId.Generate().Value,
                ParticipantId.Generate().Value,
                "name",
                "philosophy",
                new CharterRecord(
                    new ContentionRulesRecord(0.5m, TimeSpan.FromDays(3)),
                    new MembershipRulesRecord(JoinPolicy.Open.Value, true, null, null, null),
                    new ShunningRulesRecord(0.6m)));
        }
    }
}
