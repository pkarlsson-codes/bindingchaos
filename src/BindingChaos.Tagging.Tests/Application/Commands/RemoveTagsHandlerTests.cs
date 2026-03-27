using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Tagging.Application.Commands;
using BindingChaos.Tagging.Application.Services;
using BindingChaos.Tagging.Domain.TaggableTargets;
using BindingChaos.Tagging.Domain.Tags;
using FluentAssertions;
using Moq;

namespace BindingChaos.Tagging.Tests.Application.Commands;

public class RemoveTagsHandlerTests
{
    public class TestBed
    {
        public Mock<ITaggableTargetRepository> TargetRepository { get; } = new();
        public Mock<ITagResolver> TagResolver { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        private static readonly ParticipantId TestActor = ParticipantId.Generate();

        private static TaggableTarget CreateExistingTarget()
        {
            var id = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            return new TaggableTarget(id);
        }

        [Fact]
        public async Task GivenTargetNotFound_WhenHandled_ThenThrows()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            testBed.TargetRepository
                .Setup(r => r.GetByIdOrThrowAsync(targetId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(TaggableTarget), targetId));
            var cmd = new RemoveTags(targetId, TestActor, ["environment"], DateTimeOffset.UtcNow);

            var act = async () => await RemoveTagsHandler.Handle(
                cmd,
                testBed.TagResolver.Object,
                testBed.TargetRepository.Object,
                testBed.UnitOfWork.Object);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenValidRequest_WhenHandled_ThenStagesTarget()
        {
            var target = CreateExistingTarget();
            var resolvedTagId = TagId.Generate();
            target.AssignTags([resolvedTagId], TestActor);
            target.UncommittedEvents.MarkAsCommitted();

            testBed.TargetRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<TaggableTargetId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(target);
            testBed.TagResolver
                .Setup(r => r.ResolveOrCreate(It.IsAny<string[]>(), TestActor, It.IsAny<CancellationToken>()))
                .ReturnsAsync([resolvedTagId]);
            var cmd = new RemoveTags(target.Id, TestActor, ["environment"], DateTimeOffset.UtcNow);

            await RemoveTagsHandler.Handle(
                cmd,
                testBed.TagResolver.Object,
                testBed.TargetRepository.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.TargetRepository.Verify(r => r.Stage(target), Times.Once);
        }

        [Fact]
        public async Task GivenValidRequest_WhenHandled_ThenCommitsUnitOfWork()
        {
            var target = CreateExistingTarget();
            var resolvedTagId = TagId.Generate();
            target.AssignTags([resolvedTagId], TestActor);
            target.UncommittedEvents.MarkAsCommitted();

            testBed.TargetRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<TaggableTargetId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(target);
            testBed.TagResolver
                .Setup(r => r.ResolveOrCreate(It.IsAny<string[]>(), TestActor, It.IsAny<CancellationToken>()))
                .ReturnsAsync([resolvedTagId]);
            var cmd = new RemoveTags(target.Id, TestActor, ["environment"], DateTimeOffset.UtcNow);

            await RemoveTagsHandler.Handle(
                cmd,
                testBed.TagResolver.Object,
                testBed.TargetRepository.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenEmptyLabels_WhenHandled_ThenDoesNotCommit()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            var cmd = new RemoveTags(targetId, TestActor, [], DateTimeOffset.UtcNow);

            await RemoveTagsHandler.Handle(
                cmd,
                testBed.TagResolver.Object,
                testBed.TargetRepository.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GivenAllWhitespaceLabels_WhenHandled_ThenDoesNotCommit()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            var cmd = new RemoveTags(targetId, TestActor, ["   ", "  "], DateTimeOffset.UtcNow);

            await RemoveTagsHandler.Handle(
                cmd,
                testBed.TagResolver.Object,
                testBed.TargetRepository.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
