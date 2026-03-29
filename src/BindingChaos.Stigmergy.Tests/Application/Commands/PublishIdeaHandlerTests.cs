using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Ideas;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class PublishIdeaHandlerTests
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
            var command = new PublishIdea(IdeaId.Generate(), ParticipantId.Generate());

            var act = async () => await PublishIdeaHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenDraftIdeaAndAuthor_WhenHandled_ThenStagesIdeaAndCommits()
        {
            var authorId = ParticipantId.Generate();
            var idea = Idea.Draft(authorId, "A Better Approach", "Description of the idea");
            idea.UncommittedEvents.MarkAsCommitted();
            testBed.IdeaRepository
                .Setup(r => r.GetByIdOrThrowAsync(idea.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new PublishIdea(idea.Id, authorId);

            await PublishIdeaHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.IdeaRepository.Verify(r => r.Stage(It.IsAny<Idea>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
