using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Application.Commands;
using BindingChaos.SignalAwareness.Domain.Evidence;
using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;
using Moq;
using AddEvidenceCommand = BindingChaos.SignalAwareness.Application.Commands.AddEvidence;

namespace BindingChaos.SignalAwareness.Tests.Application.Commands.AddEvidence;

public class AddEvidenceHandlerTests
{
    private class TestBed
    {
        public Mock<ISignalRepository> SignalRepository { get; } = new();
        public Mock<IEvidenceRepository> EvidenceRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenSignalNotFound_WhenHandled_ThenThrowsException()
        {
            var signalId = SignalId.Generate();
            testBed.SignalRepository
                .Setup(r => r.ExistsByIdAsync(signalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var command = new AddEvidenceCommand(signalId, ["doc-1"], "Some evidence", ParticipantId.Generate());

            var act = async () => await AddEvidenceHandler.Handle(
                command,
                testBed.SignalRepository.Object,
                testBed.EvidenceRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenSignalExists_WhenHandled_ThenStagesEvidence()
        {
            var signalId = SignalId.Generate();
            testBed.SignalRepository
                .Setup(r => r.ExistsByIdAsync(signalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var command = new AddEvidenceCommand(signalId, ["doc-1"], "Some evidence", ParticipantId.Generate());

            await AddEvidenceHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.EvidenceRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.EvidenceRepository.Verify(r => r.Stage(It.IsAny<BindingChaos.SignalAwareness.Domain.Evidence.Evidence>()), Times.Once);
        }

        [Fact]
        public async Task GivenSignalExists_WhenHandled_ThenCommitsUnitOfWork()
        {
            var signalId = SignalId.Generate();
            testBed.SignalRepository
                .Setup(r => r.ExistsByIdAsync(signalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var command = new AddEvidenceCommand(signalId, ["doc-1"], "Some evidence", ParticipantId.Generate());

            await AddEvidenceHandler.Handle(command, testBed.SignalRepository.Object,
                testBed.EvidenceRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
