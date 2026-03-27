using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Application.Commands;
using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;
using Moq;
using AmplifySignalCommand = BindingChaos.SignalAwareness.Application.Commands.AmplifySignal;

namespace BindingChaos.SignalAwareness.Tests.Application.Commands.AmplifySignal;

public class AmplifySignalHandlerTests
{
    private class TestBed
    {
        public Mock<ISignalRepository> SignalRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        private static Signal CreateSignal() =>
            Signal.Capture(
                SignalContent.Create("Test Signal", "A description"),
                ParticipantId.Generate(),
                null,
                [],
                []);

        [Fact]
        public async Task GivenSignalNotFound_WhenHandled_ThenThrowsException()
        {
            var signalId = SignalId.Generate();
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signalId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Signal), signalId));
            var command = new AmplifySignalCommand(signalId, ParticipantId.Generate(), AmplificationReason.HighRelevance, null);

            var act = async () => await AmplifySignalHandler.Handle(
                command,
                testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenSignalIsAmplified()
        {
            var signal = CreateSignal();
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var amplifierId = ParticipantId.Generate();
            var command = new AmplifySignalCommand(signal.Id, amplifierId, AmplificationReason.HighRelevance, null);

            var result = await AmplifySignalHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            result.Should().Be(1);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenStagesSignal()
        {
            var signal = CreateSignal();
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new AmplifySignalCommand(signal.Id, ParticipantId.Generate(), AmplificationReason.HighRelevance, null);

            await AmplifySignalHandler.Handle(
                command,
                testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            testBed.SignalRepository.Verify(r => r.Stage(signal), Times.Once);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenCommitsUnitOfWork()
        {
            var signal = CreateSignal();
            testBed.SignalRepository
                .Setup(r => r.GetByIdOrThrowAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new AmplifySignalCommand(signal.Id, ParticipantId.Generate(), AmplificationReason.HighRelevance, null);

            await AmplifySignalHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
