using BindingChaos.Reputation.Application.Commands;
using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace BindingChaos.Reputation.Tests.Application.Commands;

public class ExtendTrustHandlerTests
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
        public async Task GivenValidCommand_WhenHandled_ThenCallsTrustAsyncOnRepository()
        {
            _testBed.Repository
                .Setup(r => r.TrustAsync(It.IsAny<TrustRelationship>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await ExtendTrustHandler.Handle(
                new ExtendTrust(_trusterId, _trusteeId),
                _testBed.Repository.Object,
                CancellationToken.None);

            _testBed.Repository.Verify(
                r => r.TrustAsync(
                    It.Is<TrustRelationship>(rel => rel.TrusterId == _trusterId && rel.TrusteeId == _trusteeId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenSelfTrust_WhenHandled_ThenThrowsInvariantViolationException()
        {
            var selfId = ParticipantId.Generate();

            var act = async () => await ExtendTrustHandler.Handle(
                new ExtendTrust(selfId, selfId),
                _testBed.Repository.Object,
                CancellationToken.None);

            await act.Should().ThrowAsync<InvariantViolationException>();
            _testBed.Repository.Verify(
                r => r.TrustAsync(It.IsAny<TrustRelationship>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
