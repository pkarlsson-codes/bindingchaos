using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Ideas;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class UpdateIdeaDraftHandlerTests
{
    private class TestBed
    {
        public Mock<IIdeaRepository> IdeaRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenIdeaNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.IdeaRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Idea), IdeaId.Generate()));
            var command = new UpdateIdeaDraft(IdeaId.Generate(), ParticipantId.Generate(), "New Title", "New Description");

            var act = async () => await UpdateIdeaDraftHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenDraftIdeaAndAuthor_WhenHandled_ThenStagesIdeaAndCommits()
        {
            var authorId = ParticipantId.Generate();
            var idea = Idea.Draft(authorId, "Original Title", "Original Description");
            idea.UncommittedEvents.MarkAsCommitted();
            testBed.IdeaRepository
                .Setup(r => r.GetByIdOrThrowAsync(idea.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new UpdateIdeaDraft(idea.Id, authorId, "Updated Title", "Updated Description");

            await UpdateIdeaDraftHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.IdeaRepository.Verify(r => r.Stage(It.IsAny<Idea>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
