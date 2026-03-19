using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Application.Commands;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;
using FluentAssertions;
using Moq;
using SuggestActionCommand = BindingChaos.SignalAwareness.Application.Commands.SuggestAction;

namespace BindingChaos.SignalAwareness.Tests.Application.Commands.SuggestAction;

public class SuggestActionHandlerTests
{
    private class TestBed
    {
        public Mock<ISignalRepository> SignalRepository { get; } = new();
        public Mock<ISignalActionRepository> SignalActionRepository { get; } = new();
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
                .Setup(r => r.GetByIdAsync(signalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Signal?)null);
            var command = new SuggestActionCommand(signalId, ParticipantId.Generate(), new MakeACallParameters("555-1234"));

            var act = async () => await SuggestActionHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.SignalActionRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenMakeACallParameters_WhenHandled_ThenStagesAction()
        {
            var signal = CreateSignal();
            testBed.SignalRepository
                .Setup(r => r.GetByIdAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new SuggestActionCommand(signal.Id, ParticipantId.Generate(), new MakeACallParameters("555-1234"));

            await SuggestActionHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.SignalActionRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.SignalActionRepository.Verify(r => r.Stage(It.IsAny<BindingChaos.SignalAwareness.Domain.SuggestedActions.SuggestedAction>()), Times.Once);
        }

        [Fact]
        public async Task GivenVisitAWebpageParameters_WhenHandled_ThenStagesAction()
        {
            var signal = CreateSignal();
            testBed.SignalRepository
                .Setup(r => r.GetByIdAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new SuggestActionCommand(signal.Id, ParticipantId.Generate(), new VisitAWebpageParameters("https://example.com"));

            await SuggestActionHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.SignalActionRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.SignalActionRepository.Verify(r => r.Stage(It.IsAny<BindingChaos.SignalAwareness.Domain.SuggestedActions.SuggestedAction>()), Times.Once);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenCommitsUnitOfWork()
        {
            var signal = CreateSignal();
            testBed.SignalRepository
                .Setup(r => r.GetByIdAsync(signal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(signal);
            var command = new SuggestActionCommand(signal.Id, ParticipantId.Generate(), new MakeACallParameters("555-1234"));

            await SuggestActionHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.SignalActionRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
