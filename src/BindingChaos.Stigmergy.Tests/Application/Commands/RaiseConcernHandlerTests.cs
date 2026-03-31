using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Concerns;
using BindingChaos.Stigmergy.Domain.Signals;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class RaiseConcernHandlerTests
{
    private class TestBed
    {
        public Mock<IConcernRepository> ConcernRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenNullCommand_WhenHandled_ThenThrowsArgumentNullException()
        {
            var act = async () => await RaiseConcernHandler.Handle(
                null!, testBed.ConcernRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenStagesConcernAndCommits()
        {
            var command = new RaiseConcern(
                ParticipantId.Generate(),
                "Housing shortage",
                ["housing", "shortage"],
                [SignalId.Generate()]);

            var result = await RaiseConcernHandler.Handle(
                command, testBed.ConcernRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.ConcernRepository.Verify(r => r.Stage(It.IsAny<Concern>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            result.Should().NotBeNull();
        }
    }
}
