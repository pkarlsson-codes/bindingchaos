using BindingChaos.Ideation.Application.Commands;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using FluentAssertions;
using Moq;

namespace BindingChaos.Ideation.Tests.Application.Commands;

public class AcceptAmendmentHandlerTests
{
    public class TestBed
    {
        public Mock<IAmendmentRepository> AmendmentRepository { get; } = new();
        public Mock<IIdeaRepository> IdeaRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        private static Idea CreateIdea(ParticipantId creatorId) =>
            Idea.Author(SocietyId.Create("society-test"), creatorId, "A title", "A body", ["signal-abc"], []);

        private static Amendment CreateAmendment(IdeaId targetIdeaId, int targetVersion) =>
            Amendment.Propose(targetIdeaId, targetVersion, ParticipantId.Generate(),
                "New title", "New body", "Amendment title", "Amendment desc");

        [Fact]
        public async Task GivenAmendmentNotFound_WhenHandled_ThenThrowsInvalidOperationException()
        {
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Amendment?)null);
            var command = new AcceptAmendment(AmendmentId.Generate(), ParticipantId.Generate());

            var act = async () => await AcceptAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.IdeaRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenIdeaNotFound_WhenHandled_ThenThrowsInvalidOperationException()
        {
            var ideaId = IdeaId.Generate();
            var amendment = CreateAmendment(ideaId, 1);
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idea?)null);
            var command = new AcceptAmendment(amendment.Id, ParticipantId.Generate());

            var act = async () => await AcceptAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.IdeaRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenActorIsNotIdeaCreator_WhenHandled_ThenThrowsUnauthorizedAccessException()
        {
            var ideaCreator = ParticipantId.Generate();
            var idea = CreateIdea(ideaCreator);
            var amendment = CreateAmendment(idea.Id, 1);
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var differentActor = ParticipantId.Generate();
            var command = new AcceptAmendment(amendment.Id, differentActor);

            var act = async () => await AcceptAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.IdeaRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GivenVersionMismatch_WhenHandled_ThenThrowsInvalidOperationException()
        {
            var ideaCreator = ParticipantId.Generate();
            var idea = CreateIdea(ideaCreator); // idea is at version 1
            var amendment = CreateAmendment(idea.Id, 2); // amendment targets version 2
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new AcceptAmendment(amendment.Id, ideaCreator);

            var act = async () => await AcceptAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.IdeaRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenAmendmentIsAccepted()
        {
            var ideaCreator = ParticipantId.Generate();
            var idea = CreateIdea(ideaCreator);
            var amendment = CreateAmendment(idea.Id, 1);
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new AcceptAmendment(amendment.Id, ideaCreator);

            await AcceptAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.IdeaRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            amendment.Status.Should().Be(AmendmentStatus.Approved);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenIdeaIsAmended()
        {
            var ideaCreator = ParticipantId.Generate();
            var idea = CreateIdea(ideaCreator);
            var amendment = CreateAmendment(idea.Id, 1);
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new AcceptAmendment(amendment.Id, ideaCreator);

            await AcceptAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.IdeaRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            idea.CurrentVersion.VersionNumber.Should().Be(2);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenCommitsUnitOfWork()
        {
            var ideaCreator = ParticipantId.Generate();
            var idea = CreateIdea(ideaCreator);
            var amendment = CreateAmendment(idea.Id, 1);
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new AcceptAmendment(amendment.Id, ideaCreator);

            await AcceptAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.IdeaRepository.Object,
                testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
