using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Signals;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class WithdrawAmplificationHandlerTests
{
    private class TestBed
    {
        public Mock<ISignalRepository> SignalRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenNullCommand_WhenHandled_ThenThrowsArgumentNullException()
        {
            var act = async () => await WithdrawAmplificationHandler.Handle(
                null!, testBed.SignalRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenSignalNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<SignalId>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Signal), SignalId.Generate()));
            var command = new WithdrawAmplification(ParticipantId.Generate(), SignalId.Generate());

            var act = async () => await WithdrawAmplificationHandler.Handle(
                command, testBed.SignalRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenAmplifiedParticipant_WhenWithdrawn_ThenCommits()
        {
            var actorId = ParticipantId.Generate();
            var signal = Signal.Capture(
                ParticipantId.Generate(), "Title", "Description", [], [], null);
            signal.Amplify(actorId);
            signal.UncommittedEvents.MarkAsCommitted();
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new WithdrawAmplification(actorId, signal.Id);

            await WithdrawAmplificationHandler.Handle(
                command, testBed.SignalRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenParticipantWhoHasNotAmplified_WhenWithdrawn_ThenThrowsBusinessRuleViolationException()
        {
            var signal = Signal.Capture(
                ParticipantId.Generate(), "Title", "Description", [], [], null);
            signal.UncommittedEvents.MarkAsCommitted();
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new WithdrawAmplification(ParticipantId.Generate(), signal.Id);

            var act = async () => await WithdrawAmplificationHandler.Handle(
                command, testBed.SignalRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }
    }
}
