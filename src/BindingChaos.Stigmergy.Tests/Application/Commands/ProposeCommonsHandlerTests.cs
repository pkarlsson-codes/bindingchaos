using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using FluentAssertions;
using Moq;
using Wolverine;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class ProposeCommonsHandlerTests
{
    private class TestBed
    {
        public Mock<ICommonsRepository> CommonsRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IMessageBus> MessageBus { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenNullCommand_WhenHandled_ThenThrowsArgumentNullException()
        {
            var act = async () => await ProposeCommonsHandler.Handle(
                null!, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, testBed.MessageBus.Object, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenStagesCommonsAndCommits()
        {
            var command = new ProposeCommons("Water Management", "Governing local water resources", ParticipantId.Generate());

            var result = await ProposeCommonsHandler.Handle(
                command, testBed.CommonsRepository.Object, testBed.UnitOfWork.Object, testBed.MessageBus.Object, CancellationToken.None);

            testBed.CommonsRepository.Verify(r => r.Stage(It.IsAny<Commons>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            result.Should().NotBeNull();
        }
    }
}
