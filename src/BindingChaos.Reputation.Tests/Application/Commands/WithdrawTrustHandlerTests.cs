using BindingChaos.Reputation.Application.Commands;
using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.SharedKernel.Domain;
using Moq;

namespace BindingChaos.Reputation.Tests.Application.Commands;

public class WithdrawTrustHandlerTests
{
    public class TestBed
    {
        public Mock<ITrustRelationshipRepository> Repository { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed _testBed = new();
        private readonly ParticipantId _trusterId = ParticipantId.Generate();
        private readonly ParticipantId _trusteeId = ParticipantId.Generate();

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenCallsWithdrawAsyncOnRepository()
        {
            _testBed.Repository
                .Setup(r => r.WithdrawAsync(It.IsAny<ParticipantId>(), It.IsAny<ParticipantId>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await WithdrawTrustHandler.Handle(
                new WithdrawTrust(_trusterId, _trusteeId),
                _testBed.Repository.Object,
                CancellationToken.None);

            _testBed.Repository.Verify(
                r => r.WithdrawAsync(_trusterId, _trusteeId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
