using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class RenameCommonsHandlerTests
{
    private class TestBed
    {
        public Mock<ICommonsRepository> CommonsRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenNullCommand_WhenHandled_ThenThrowsArgumentNullException()
        {
            var act = async () => await RenameCommonsHandler.Handle(
                null!, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenCommonsNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.CommonsRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<CommonsId>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Commons), CommonsId.Generate()));
            var command = new RenameCommons(CommonsId.Generate(), "New Name");

            var act = async () => await RenameCommonsHandler.Handle(
                command, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenStagesCommonsAndCommits()
        {
            var commons = Commons.Propose("Old Name", "Description", ParticipantId.Generate());
            commons.UncommittedEvents.MarkAsCommitted();
            testBed.CommonsRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<CommonsId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commons);
            var command = new RenameCommons(commons.Id, "New Name");

            await RenameCommonsHandler.Handle(
                command, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.CommonsRepository.Verify(r => r.Stage(It.IsAny<Commons>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
