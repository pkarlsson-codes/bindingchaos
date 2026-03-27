using BindingChaos.Ideation.Application.Commands;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using FluentAssertions;
using Moq;

namespace BindingChaos.Ideation.Tests.Application.Commands;

public class OpposeAmendmentHandlerTests
{
    public class TestBed
    {
        public Mock<IAmendmentRepository> AmendmentRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();
        private readonly ParticipantId _supporter = ParticipantId.Generate();
        private readonly ParticipantId _opponent = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public async Task GivenParticipantAlreadySupporting_WhenHandled_ThenThrowsBusinessRuleViolationException()
        {
            var amendment = CreateOpenAmendment();
            amendment.AddSupport(new Supporter(_supporter, "I support"));
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            var command = new OpposeAmendment(amendment.Id, _supporter, "Actually I oppose");

            var act = async () => await OpposeAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenOppositionIsAddedToAmendment()
        {
            var amendment = CreateOpenAmendment();
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            var command = new OpposeAmendment(amendment.Id, _opponent, "I disagree");

            await OpposeAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            amendment.Opponents.Should().ContainSingle(o => o.ParticipantId == _opponent);
        }
    }
}
