using BindingChaos.Ideation.Application.Commands;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using FluentAssertions;
using Moq;

namespace BindingChaos.Ideation.Tests.Application.Commands;

public class SupportAmendmentHandlerTests
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
        public async Task GivenParticipantAlreadyOpposing_WhenHandled_ThenThrowsBusinessRuleViolationException()
        {
            var amendment = CreateOpenAmendment();
            amendment.AddOpposition(new Opponent(_opponent, "I oppose"));
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            var command = new SupportAmendment(amendment.Id, _opponent, "I changed my mind");

            var act = async () => await SupportAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenSupportIsAddedToAmendment()
        {
            var amendment = CreateOpenAmendment();
            testBed.AmendmentRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<AmendmentId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amendment);
            var command = new SupportAmendment(amendment.Id, _supporter, "I agree");

            await SupportAmendmentHandler.Handle(
                command, testBed.AmendmentRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            amendment.Supporters.Should().ContainSingle(s => s.ParticipantId == _supporter);
        }
    }
}
