using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Tagging.Application.Commands;
using BindingChaos.Tagging.Application.Services;
using BindingChaos.Tagging.Domain.TaggableTargets;
using BindingChaos.Tagging.Domain.Tags;
using Moq;

namespace BindingChaos.Tagging.Tests.Application.Commands;

public class CreateTaggableTargetHandlerTests
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

        [Fact]
        public async Task GivenTargetAlreadyExists_WhenHandled_ThenDoesNotStageNewTarget()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            testBed.TargetRepository
                .Setup(r => r.ExistsByIdAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var cmd = new CreateTaggableTarget(targetId, TestActor);

            await CreateTaggableTargetHandler.Handle(
                cmd,
                testBed.TargetRepository.Object,
                testBed.TagResolver.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.TargetRepository.Verify(r => r.Stage(It.IsAny<TaggableTarget>()), Times.Never);
        }

        [Fact]
        public async Task GivenTargetAlreadyExists_WhenHandled_ThenDoesNotCommit()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            testBed.TargetRepository
                .Setup(r => r.ExistsByIdAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var cmd = new CreateTaggableTarget(targetId, TestActor);

            await CreateTaggableTargetHandler.Handle(
                cmd,
                testBed.TargetRepository.Object,
                testBed.TagResolver.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GivenNewTarget_WhenHandled_ThenStagesTarget()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            testBed.TargetRepository
                .Setup(r => r.ExistsByIdAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var cmd = new CreateTaggableTarget(targetId, TestActor);

            await CreateTaggableTargetHandler.Handle(
                cmd,
                testBed.TargetRepository.Object,
                testBed.TagResolver.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.TargetRepository.Verify(r => r.Stage(It.IsAny<TaggableTarget>()), Times.Once);
        }

        [Fact]
        public async Task GivenNewTarget_WhenHandled_ThenCommitsUnitOfWork()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            testBed.TargetRepository
                .Setup(r => r.ExistsByIdAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var cmd = new CreateTaggableTarget(targetId, TestActor);

            await CreateTaggableTargetHandler.Handle(
                cmd,
                testBed.TargetRepository.Object,
                testBed.TagResolver.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenNewTargetWithInitialTags_WhenHandled_ThenResolvesTagsAndStages()
        {
            var targetId = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            testBed.TargetRepository
                .Setup(r => r.ExistsByIdAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var resolvedTagId = TagId.Generate();
            testBed.TagResolver
                .Setup(r => r.ResolveOrCreate(It.IsAny<string[]>(), TestActor, It.IsAny<CancellationToken>()))
                .ReturnsAsync([resolvedTagId]);
            var cmd = new CreateTaggableTarget(targetId, TestActor, InitialTags: ["environment"]);

            await CreateTaggableTargetHandler.Handle(
                cmd,
                testBed.TargetRepository.Object,
                testBed.TagResolver.Object,
                testBed.UnitOfWork.Object,
                TestContext.Current.CancellationToken);

            testBed.TagResolver.Verify(
                r => r.ResolveOrCreate(It.IsAny<string[]>(), TestActor, It.IsAny<CancellationToken>()),
                Times.Once);
            testBed.TargetRepository.Verify(r => r.Stage(It.IsAny<TaggableTarget>()), Times.Once);
        }
    }
}
