using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Ideas;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class ForkIdeaHandlerTests
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
            var command = new ForkIdea(ParticipantId.Generate(), IdeaId.Generate(), "Fork Title", "Fork Description");

            var act = async () => await ForkIdeaHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenPublishedIdea_WhenForkedByAnyParticipant_ThenStagesForkedIdeaAndCommits()
        {
            var authorId = ParticipantId.Generate();
            var originalIdea = Idea.Draft(authorId, "Original Title", "Original Description");
            originalIdea.Publish(authorId);
            originalIdea.UncommittedEvents.MarkAsCommitted();
            testBed.IdeaRepository
                .Setup(r => r.GetByIdOrThrowAsync(originalIdea.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(originalIdea);

            var command = new ForkIdea(ParticipantId.Generate(), originalIdea.Id, "Fork Title", "Fork Description");

            await ForkIdeaHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.IdeaRepository.Verify(r => r.Stage(It.IsAny<Idea>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenDraftIdea_WhenForkedByAuthor_ThenStagesForkedIdeaAndCommits()
        {
            var authorId = ParticipantId.Generate();
            var originalIdea = Idea.Draft(authorId, "Original Title", "Original Description");
            originalIdea.UncommittedEvents.MarkAsCommitted();
            testBed.IdeaRepository
                .Setup(r => r.GetByIdOrThrowAsync(originalIdea.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(originalIdea);

            var command = new ForkIdea(authorId, originalIdea.Id, "Fork Title", "Fork Description");

            await ForkIdeaHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.IdeaRepository.Verify(r => r.Stage(It.IsAny<Idea>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenDraftIdea_WhenForkedByNonAuthor_ThenThrowsForbiddenException()
        {
            var originalIdea = Idea.Draft(ParticipantId.Generate(), "Original Title", "Original Description");
            originalIdea.UncommittedEvents.MarkAsCommitted();
            testBed.IdeaRepository
                .Setup(r => r.GetByIdOrThrowAsync(originalIdea.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(originalIdea);

            var command = new ForkIdea(ParticipantId.Generate(), originalIdea.Id, "Fork Title", "Fork Description");

            var act = async () => await ForkIdeaHandler.Handle(
                command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenException>();
        }
    }
}
