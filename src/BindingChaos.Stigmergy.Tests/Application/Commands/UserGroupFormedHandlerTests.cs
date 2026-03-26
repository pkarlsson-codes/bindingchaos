using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Application.EventHandlers;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using BindingChaos.Stigmergy.Domain.UserGroups;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;
using FluentAssertions;
using Marten;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class UserGroupFormedHandlerTests
{
    private class TestBed
    {
        public Mock<IDocumentSession> Session { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenCommonsNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.Session
                .Setup(s => s.LoadAsync<Commons>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Commons?)null);
            UserGroupFormed message = CreateUserGroupFormedEvent();

            var act = async () => await UserGroupFormedHandler.Handle(
                message, testBed.Session.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenProposedCommons_WhenHandled_ThenSavesChanges()
        {
            var commons = Commons.Propose("Water", "Governing water", ParticipantId.Generate());
            commons.UncommittedEvents.MarkAsCommitted();
            List<IDomainEvent> events = [];
            testBed.Session
                .Setup(s => s.LoadAsync<Commons>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commons);
            testBed.Session
                .Setup(s => s.Store(It.IsAny<Commons>()))
                    .Callback<Commons[]>(entities => events.AddRange(entities[0].UncommittedEvents));
            var message = CreateUserGroupFormedEvent();

            await UserGroupFormedHandler.Handle(message, testBed.Session.Object, CancellationToken.None);

            testBed.Session.Verify(s => s.Store(It.IsAny<Commons>()), Times.Once);
            testBed.Session.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
