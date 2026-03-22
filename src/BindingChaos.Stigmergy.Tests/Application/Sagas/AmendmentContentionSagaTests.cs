using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Application.Sagas;
using FluentAssertions;
using Moq;
using Wolverine;

namespace BindingChaos.Stigmergy.Tests.Application.Sagas;

public class AmendmentContentionSagaTests
{
    private static AmendmentContentionStarted AStartMessage(
        string amendmentId = "amendment-001",
        string projectId = "project-001",
        decimal rejectionThreshold = 0.5m,
        TimeSpan? resolutionWindow = null,
        string contesterId = "participant-001") =>
        new(
            amendmentId,
            projectId,
            "usergroup-001",
            rejectionThreshold,
            resolutionWindow ?? TimeSpan.FromDays(3),
            contesterId);

    public class TheStartMethod
    {
        [Fact]
        public async Task GivenStartMessage_WhenStarted_ThenSagaIdIsAmendmentId()
        {
            var message = AStartMessage(amendmentId: "amendment-abc");
            var mockContext = new Mock<IMessageContext>();

            var saga = await AmendmentContentionSaga.Start(message, mockContext.Object);

            saga.Id.Should().Be("amendment-abc");
        }

        [Fact]
        public async Task GivenStartMessage_WhenStarted_ThenContesterRecordedAsAgreeVote()
        {
            var message = AStartMessage(contesterId: "participant-xyz");
            var mockContext = new Mock<IMessageContext>();

            var saga = await AmendmentContentionSaga.Start(message, mockContext.Object);

            saga.Votes.Should().ContainKey("participant-xyz");
            saga.Votes["participant-xyz"].Should().BeTrue();
        }

        [Fact]
        public async Task GivenStartMessage_WhenStarted_ThenRejectionThresholdSnapshotted()
        {
            var message = AStartMessage(rejectionThreshold: 0.75m);
            var mockContext = new Mock<IMessageContext>();

            var saga = await AmendmentContentionSaga.Start(message, mockContext.Object);

            saga.RejectionThreshold.Should().Be(0.75m);
        }

        [Fact]
        public async Task GivenStartMessage_WhenStarted_ThenSchedulesResolution()
        {
            var resolutionWindow = TimeSpan.FromDays(7);
            var message = AStartMessage(resolutionWindow: resolutionWindow);
            var mockContext = new Mock<IMessageContext>();

            await AmendmentContentionSaga.Start(message, mockContext.Object);

            // ScheduleAsync is a Wolverine extension method that delegates to SendAsync with ScheduleDelay set.
            // We verify the underlying SendAsync call on the interface directly.
            mockContext.Verify(
                c => c.SendAsync(
                    It.Is<ResolveAmendmentContention>(m => m.AmendmentContentionSagaId == message.AmendmentId),
                    It.Is<DeliveryOptions?>(o => o != null && o.ScheduleDelay == resolutionWindow)),
                Times.Once);
        }
    }

    public class TheHandleInteractionMethod
    {
        [Fact]
        public async Task GivenNewParticipant_WhenHandled_ThenVoteRecorded()
        {
            var message = AStartMessage();
            var saga = await AmendmentContentionSaga.Start(message, Mock.Of<IMessageContext>());

            saga.Handle(new InteractWithAmendmentContention("amendment-001", "participant-002", true));

            saga.Votes.Should().ContainKey("participant-002");
            saga.Votes["participant-002"].Should().BeTrue();
        }

        [Fact]
        public async Task GivenExistingVote_WhenHandledWithDifferentVote_ThenVoteUpdated()
        {
            var message = AStartMessage(contesterId: "participant-001");
            var saga = await AmendmentContentionSaga.Start(message, Mock.Of<IMessageContext>());

            saga.Handle(new InteractWithAmendmentContention("amendment-001", "participant-001", false));

            saga.Votes["participant-001"].Should().BeFalse();
        }
    }

    public class TheHandleResolutionMethod
    {
        [Fact]
        public async Task GivenRatioAboveThreshold_WhenResolved_ThenInvokesRejectAmendment()
        {
            // 2 agree, 1 disagree = 0.67 ratio >= 0.5 threshold → reject
            var message = AStartMessage(contesterId: "p1", rejectionThreshold: 0.5m);
            var saga = await AmendmentContentionSaga.Start(message, Mock.Of<IMessageContext>());
            saga.Handle(new InteractWithAmendmentContention("amendment-001", "p2", true));
            saga.Handle(new InteractWithAmendmentContention("amendment-001", "p3", false));

            var mockBus = new Mock<IMessageBus>();
            await saga.Handle(new ResolveAmendmentContention("amendment-001", "project-001"), mockBus.Object);

            mockBus.Verify(
                b => b.InvokeAsync(It.IsAny<RejectAmendment>(), It.IsAny<CancellationToken>()),
                Times.Once);
            mockBus.Verify(
                b => b.InvokeAsync(It.IsAny<RestoreAmendmentToActive>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task GivenRatioBelowThreshold_WhenResolved_ThenInvokesRestoreAmendmentToActive()
        {
            // 1 agree, 2 disagree = 0.33 ratio < 0.5 threshold → restore
            var message = AStartMessage(contesterId: "p1", rejectionThreshold: 0.5m);
            var saga = await AmendmentContentionSaga.Start(message, Mock.Of<IMessageContext>());
            saga.Handle(new InteractWithAmendmentContention("amendment-001", "p2", false));
            saga.Handle(new InteractWithAmendmentContention("amendment-001", "p3", false));

            var mockBus = new Mock<IMessageBus>();
            await saga.Handle(new ResolveAmendmentContention("amendment-001", "project-001"), mockBus.Object);

            mockBus.Verify(
                b => b.InvokeAsync(It.IsAny<RestoreAmendmentToActive>(), It.IsAny<CancellationToken>()),
                Times.Once);
            mockBus.Verify(
                b => b.InvokeAsync(It.IsAny<RejectAmendment>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task GivenNoInteractions_WhenResolved_ThenRestoresAmendmentToActive()
        {
            // Zero votes → denominator 0 → restores to active (silence = acceptance)
            var message = AStartMessage(contesterId: "p1", rejectionThreshold: 0.5m);
            var saga = await AmendmentContentionSaga.Start(message, Mock.Of<IMessageContext>());
            saga.Votes.Clear();

            var mockBus = new Mock<IMessageBus>();
            await saga.Handle(new ResolveAmendmentContention("amendment-001", "project-001"), mockBus.Object);

            mockBus.Verify(
                b => b.InvokeAsync(It.IsAny<RestoreAmendmentToActive>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenResolution_WhenHandled_ThenSagaCompleted()
        {
            var message = AStartMessage();
            var saga = await AmendmentContentionSaga.Start(message, Mock.Of<IMessageContext>());

            await saga.Handle(new ResolveAmendmentContention("amendment-001", "project-001"), Mock.Of<IMessageBus>());

            saga.IsCompleted().Should().BeTrue();
        }
    }
}
