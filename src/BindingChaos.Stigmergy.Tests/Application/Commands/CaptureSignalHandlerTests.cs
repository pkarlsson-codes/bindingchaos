using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Signals;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

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

        [Fact]
        public async Task GivenNullCommand_WhenHandled_ThenThrowsArgumentNullException()
        {
            var act = async () => await CaptureSignalHandler.Handle(
                null!, testBed.SignalRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenStagesSignalAndCommits()
        {
            var command = new CaptureSignal(
                ParticipantId.Generate(),
                "Signal title",
                "Something is happening",
                ["tag1", "tag2"],
                ["attachment1"],
                new Coordinates(1, 2));

            var result = await CaptureSignalHandler.Handle(
                command, testBed.SignalRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.SignalRepository.Verify(r => r.Stage(It.IsAny<Signal>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            result.Should().NotBeNull();
        }
    }
}
