using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Application.Commands;
using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;
using Moq;
using DeamplifySignalCommand = BindingChaos.SignalAwareness.Application.Commands.DeamplifySignal;

namespace BindingChaos.SignalAwareness.Tests.Application.Commands.DeamplifySignal;

public class DeamplifySignalHandlerTests
{
    private class TestBed
    {
        public Mock<ISignalRepository> SignalRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        private static Signal CreateSignalWithAmplification(out ParticipantId amplifierId)
        {
            amplifierId = ParticipantId.Generate();
            var signal = Signal.Capture(
                SignalContent.Create("Test Signal", "A description"),
                ParticipantId.Generate(),
                null,
                [],
                []);
            signal.Amplify(amplifierId, AmplificationReason.HighRelevance);
            signal.UncommittedEvents.MarkAsCommitted();
            return signal;
        }

        [Fact]
        public async Task GivenSignalNotFound_WhenHandled_ThenThrowsException()
        {
            var signalId = SignalId.Generate();
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signalId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Signal), signalId));
            var command = new DeamplifySignalCommand(signalId, ParticipantId.Generate());

            var act = async () => await DeamplifySignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenParticipantWithActiveAmplification_WhenHandled_ThenAmplificationIsAttenuated()
        {
            var signal = CreateSignalWithAmplification(out var amplifierId);
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new DeamplifySignalCommand(signal.Id, amplifierId);

            await DeamplifySignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            signal.ActiveAmplifications.Should().BeEmpty();
        }

        [Fact]
        public async Task GivenParticipantWithActiveAmplification_WhenHandled_ThenStagesSignal()
        {
            var signal = CreateSignalWithAmplification(out var amplifierId);
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new DeamplifySignalCommand(signal.Id, amplifierId);

            await DeamplifySignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.SignalRepository.Verify(r => r.Stage(signal), Times.Once);
        }

        [Fact]
        public async Task GivenParticipantWithActiveAmplification_WhenHandled_ThenCommitsUnitOfWork()
        {
            var signal = CreateSignalWithAmplification(out var amplifierId);
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new DeamplifySignalCommand(signal.Id, amplifierId);

            await DeamplifySignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenParticipantWithActiveAmplification_WhenHandled_ThenReturnsActiveCount()
        {
            var signal = CreateSignalWithAmplification(out var amplifierId);
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new DeamplifySignalCommand(signal.Id, amplifierId);

            var result = await DeamplifySignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}
