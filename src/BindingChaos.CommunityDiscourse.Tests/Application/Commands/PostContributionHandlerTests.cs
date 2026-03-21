using BindingChaos.CommunityDiscourse.Application.Commands;
using BindingChaos.CommunityDiscourse.Domain.Contributions;
using BindingChaos.CommunityDiscourse.Domain.Contributions.Events;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using FluentAssertions;
using Moq;

namespace BindingChaos.CommunityDiscourse.Tests.Application.Commands;

public class PostContributionHandlerTests
{
    public class TestBed
    {
        public Mock<IDiscourseThreadRepository> ThreadRepository { get; } = new();
        public Mock<IContributionRepository> ContributionRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();

        public TestBed()
        {
            UnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        private static DiscourseThread CreateThread()
        {
            var entityReference = EntityReference.Create("idea", "idea-abc123");
            return DiscourseThread.Create(entityReference);
        }

        [Fact]
        public async Task GivenThreadNotFound_WhenHandled_ThenThrowsInvalidOperationException()
        {
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DiscourseThread?)null);
            var threadId = DiscourseThreadId.Generate();
            var command = new PostContribution(threadId.Value, ParticipantId.Generate(), "Some content");

            var act = async () => await PostContributionHandler.Handle(
                command,
                testBed.ThreadRepository.Object,
                testBed.ContributionRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenThreadNotFound_WhenHandled_ThenDoesNotStageContribution()
        {
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DiscourseThread?)null);
            var threadId = DiscourseThreadId.Generate();
            var command = new PostContribution(threadId.Value, ParticipantId.Generate(), "Some content");

            try
            {
                await PostContributionHandler.Handle(
                    command,
                    testBed.ThreadRepository.Object,
                    testBed.ContributionRepository.Object,
                    testBed.UnitOfWork.Object,
                    CancellationToken.None);
            }
            catch (InvalidOperationException)
            {
            }

            testBed.ContributionRepository.Verify(r => r.Stage(It.IsAny<Contribution>()), Times.Never);
        }

        [Fact]
        public async Task GivenThreadExists_WhenHandled_ThenStagesContribution()
        {
            var thread = CreateThread();
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(thread);
            var command = new PostContribution(thread.Id.Value, ParticipantId.Generate(), "Some content");

            await PostContributionHandler.Handle(
                command,
                testBed.ThreadRepository.Object,
                testBed.ContributionRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            testBed.ContributionRepository.Verify(r => r.Stage(It.IsAny<Contribution>()), Times.Once);
        }

        [Fact]
        public async Task GivenThreadExists_WhenHandled_ThenCommitsUnitOfWork()
        {
            var thread = CreateThread();
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(thread);
            var command = new PostContribution(thread.Id.Value, ParticipantId.Generate(), "Some content");

            await PostContributionHandler.Handle(
                command,
                testBed.ThreadRepository.Object,
                testBed.ContributionRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenThreadExists_WhenHandled_ThenReturnsContributionId()
        {
            var thread = CreateThread();
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(thread);
            var command = new PostContribution(thread.Id.Value, ParticipantId.Generate(), "Some content");

            var result = await PostContributionHandler.Handle(
                command,
                testBed.ThreadRepository.Object,
                testBed.ContributionRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            result.Should().NotBeNull();
            result.Value.Should().StartWith("contribution-");
        }

        [Fact]
        public async Task GivenThreadExistsAndParentContributionIdProvided_WhenHandled_ThenStagedContributionCarriesParentId()
        {
            var thread = CreateThread();
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(thread);
            var parentId = ContributionId.Generate();
            Contribution? staged = null;
            testBed.ContributionRepository
                .Setup(r => r.Stage(It.IsAny<Contribution>()))
                .Callback<Contribution>(c => staged = c);
            var command = new PostContribution(thread.Id.Value, ParticipantId.Generate(), "A reply", parentId);

            await PostContributionHandler.Handle(
                command,
                testBed.ThreadRepository.Object,
                testBed.ContributionRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            staged.Should().NotBeNull();
            staged!.UncommittedEvents.OfType<ContributionAdded>().Single().ParentContributionId.Should().Be(parentId.Value);
        }

        [Fact]
        public async Task GivenThreadExistsAndNoParentContributionId_WhenHandled_ThenStagedContributionHasNullParentId()
        {
            var thread = CreateThread();
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(thread);
            Contribution? staged = null;
            testBed.ContributionRepository
                .Setup(r => r.Stage(It.IsAny<Contribution>()))
                .Callback<Contribution>(c => staged = c);
            var command = new PostContribution(thread.Id.Value, ParticipantId.Generate(), "A root contribution");

            await PostContributionHandler.Handle(
                command,
                testBed.ThreadRepository.Object,
                testBed.ContributionRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            staged.Should().NotBeNull();
            staged!.UncommittedEvents.OfType<ContributionAdded>().Single().ParentContributionId.Should().BeNull();
        }

        [Fact]
        public async Task GivenThreadExists_WhenHandled_ThenStagedContributionHasCorrectAuthorId()
        {
            var thread = CreateThread();
            testBed.ThreadRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<DiscourseThreadId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(thread);
            var authorId = ParticipantId.Generate();
            Contribution? staged = null;
            testBed.ContributionRepository
                .Setup(r => r.Stage(It.IsAny<Contribution>()))
                .Callback<Contribution>(c => staged = c);
            var command = new PostContribution(thread.Id.Value, authorId, "Some content");

            await PostContributionHandler.Handle(
                command,
                testBed.ThreadRepository.Object,
                testBed.ContributionRepository.Object,
                testBed.UnitOfWork.Object,
                CancellationToken.None);

            staged.Should().NotBeNull();
            staged!.UncommittedEvents.OfType<ContributionAdded>().Single().AuthorId.Should().Be(authorId.Value);
        }
    }
}
