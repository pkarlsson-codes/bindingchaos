using BindingChaos.Ideation.Application.Commands;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Amendments.Events;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Wolverine;

namespace BindingChaos.Ideation.Tests.Application.Commands;

public class AmendIdeaHandlerTests
{
    public class TestBed
    {
        public Mock<IIdeaRepository> IdeaRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IMessageBus> MessageBus { get; } = new();
        public Mock<ILogger> Logger { get; } = new();

        public TestBed()
        {
            MessageBus.Setup(m => m.PublishAsync(It.IsAny<AmendmentApplicationFailed>(), null))
                .Returns(ValueTask.CompletedTask);
        }
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        private static Idea CreateIdea() =>
            Idea.Author(SocietyId.Create("society-test"), ParticipantId.Generate(), "A title", "A body", ["signal-abc"], []);

        [Fact]
        public async Task GivenIdeaNotFound_WhenHandled_ThenPublishesAmendmentApplicationFailedWithIdeaNotFoundReason()
        {
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idea?)null);
            AmendmentApplicationFailed? captured = null;
            testBed.MessageBus
                .Setup(m => m.PublishAsync(It.IsAny<AmendmentApplicationFailed>(), null))
                .Callback<AmendmentApplicationFailed, DeliveryOptions?>((msg, _) => captured = msg)
                .Returns(ValueTask.CompletedTask);
            var command = new AmendIdea(IdeaId.Generate(), AmendmentId.Generate(), "New title", "New body", 1);

            await AmendIdeaHandler.Handle(command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object,
                testBed.MessageBus.Object, testBed.Logger.Object, CancellationToken.None);

            captured.Should().NotBeNull();
            captured!.Reason.Should().Be("IdeaNotFound");
        }

        [Fact]
        public async Task GivenIdeaNotFound_WhenHandled_ThenDoesNotCommit()
        {
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idea?)null);
            var command = new AmendIdea(IdeaId.Generate(), AmendmentId.Generate(), "New title", "New body", 1);

            await AmendIdeaHandler.Handle(command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object,
                testBed.MessageBus.Object, testBed.Logger.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GivenVersionMismatch_WhenHandled_ThenPublishesAmendmentApplicationFailedWithVersionMismatchReason()
        {
            var idea = CreateIdea(); // version 1
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            AmendmentApplicationFailed? captured = null;
            testBed.MessageBus
                .Setup(m => m.PublishAsync(It.IsAny<AmendmentApplicationFailed>(), null))
                .Callback<AmendmentApplicationFailed, DeliveryOptions?>((msg, _) => captured = msg)
                .Returns(ValueTask.CompletedTask);
            var command = new AmendIdea(idea.Id, AmendmentId.Generate(), "New title", "New body", ExpectedVersion: 2);

            await AmendIdeaHandler.Handle(command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object,
                testBed.MessageBus.Object, testBed.Logger.Object, CancellationToken.None);

            captured.Should().NotBeNull();
            captured!.Reason.Should().Be("VersionMismatch");
        }

        [Fact]
        public async Task GivenVersionMismatch_WhenHandled_ThenIncludesActualVersionInFailureEvent()
        {
            var idea = CreateIdea(); // version 1
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            AmendmentApplicationFailed? captured = null;
            testBed.MessageBus
                .Setup(m => m.PublishAsync(It.IsAny<AmendmentApplicationFailed>(), null))
                .Callback<AmendmentApplicationFailed, DeliveryOptions?>((msg, _) => captured = msg)
                .Returns(ValueTask.CompletedTask);
            var command = new AmendIdea(idea.Id, AmendmentId.Generate(), "New title", "New body", ExpectedVersion: 2);

            await AmendIdeaHandler.Handle(command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object,
                testBed.MessageBus.Object, testBed.Logger.Object, CancellationToken.None);

            captured!.IdeaCurrentVersion.Should().Be(1);
        }

        [Fact]
        public async Task GivenVersionMismatch_WhenHandled_ThenDoesNotCommit()
        {
            var idea = CreateIdea(); // version 1
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new AmendIdea(idea.Id, AmendmentId.Generate(), "New title", "New body", ExpectedVersion: 2);

            await AmendIdeaHandler.Handle(command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object,
                testBed.MessageBus.Object, testBed.Logger.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GivenVersionMatch_WhenHandled_ThenIdeaVersionIncreases()
        {
            var idea = CreateIdea(); // version 1
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new AmendIdea(idea.Id, AmendmentId.Generate(), "New title", "New body", ExpectedVersion: 1);

            await AmendIdeaHandler.Handle(command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object,
                testBed.MessageBus.Object, testBed.Logger.Object, CancellationToken.None);

            idea.CurrentVersion.VersionNumber.Should().Be(2);
        }

        [Fact]
        public async Task GivenVersionMatch_WhenHandled_ThenCommitsUnitOfWork()
        {
            var idea = CreateIdea(); // version 1
            testBed.IdeaRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<IdeaId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idea);
            var command = new AmendIdea(idea.Id, AmendmentId.Generate(), "New title", "New body", ExpectedVersion: 1);

            await AmendIdeaHandler.Handle(command, testBed.IdeaRepository.Object, testBed.UnitOfWork.Object,
                testBed.MessageBus.Object, testBed.Logger.Object, CancellationToken.None);

            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
