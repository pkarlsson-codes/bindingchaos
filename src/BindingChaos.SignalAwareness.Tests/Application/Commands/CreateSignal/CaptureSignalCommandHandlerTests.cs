using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Application.Commands;
using BindingChaos.SignalAwareness.Application.DTOs;
using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;
using Moq;

namespace BindingChaos.SignalAwareness.Tests.Application.Commands.CreateSignal;

public class CaptureSignalHandlerTests
{
    private class TestBed
    {
        public Mock<ISignalRepository> SignalRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        private static CaptureSignal CreateValidCommand() =>
            new(
                "Test Signal",
                "Test Description",
                ParticipantId.Generate(),
                null,
                [],
                []);

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenStagesSignalOnRepository()
        {
            var command = CreateValidCommand();

            await CaptureSignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.SignalRepository.Verify(r => r.Stage(It.IsAny<Signal>()), Times.Once);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenCommitsUnitOfWork()
        {
            var command = CreateValidCommand();

            await CaptureSignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenReturnsSignalId()
        {
            var command = CreateValidCommand();

            var result = await CaptureSignalCommandHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            result.Should().NotBeNull();
            result.Value.Should().MatchRegex(@"^signal-[0-9a-hjkmnp-tv-z]{26}$");
        }
    }
}
