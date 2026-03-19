using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Amendments.Events;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using FluentAssertions;

namespace BindingChaos.Ideation.Tests.Domain.Amendments;

public class AmendmentTests
{
    private readonly IdeaId _targetIdeaId = IdeaId.Generate();
    private readonly ParticipantId _creator = ParticipantId.Generate();
    private const int TargetVersionNumber = 1;
    private const string ProposedTitle = "Proposed Title";
    private const string ProposedBody = "Proposed body content";
    private const string AmendmentTitle = "Amendment Title";
    private const string AmendmentDescription = "Amendment description";

    private Amendment CreateOpenAmendment() =>
        Amendment.Propose(
            _targetIdeaId,
            TargetVersionNumber,
            _creator,
            ProposedTitle,
            ProposedBody,
            AmendmentTitle,
            AmendmentDescription);

    private Amendment CreateOutdatedAmendment()
    {
        var amendment = CreateOpenAmendment();
        amendment.MarkOutdated(TargetVersionNumber + 1);
        amendment.UncommittedEvents.MarkAsCommitted();
        return amendment;
    }

    [Fact]
    public void Reject_WhenOpen_ShouldSetStatusToRejected()
    {
        var amendment = CreateOpenAmendment();
        amendment.UncommittedEvents.MarkAsCommitted();

        amendment.Reject();

        amendment.Status.Should().Be(AmendmentStatus.Rejected);
    }

    [Fact]
    public void Reject_WhenOpen_ShouldSetRejectedAt()
    {
        var amendment = CreateOpenAmendment();
        amendment.UncommittedEvents.MarkAsCommitted();

        amendment.Reject();

        amendment.RejectedAt.Should().NotBeNull();
        amendment.RejectedAt!.Value.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Reject_WhenOpen_ShouldRaiseAmendmentRejectedEvent()
    {
        var amendment = CreateOpenAmendment();
        amendment.UncommittedEvents.MarkAsCommitted();

        amendment.Reject();

        amendment.UncommittedEvents.Should().HaveCount(1);
        var evt = amendment.UncommittedEvents.First().Should().BeOfType<AmendmentRejected>().Subject;
        evt.TargetIdeaId.Should().Be(_targetIdeaId.Value);
    }

    [Fact]
    public void Reject_WhenAlreadyRejected_ShouldThrowBusinessRuleViolationException()
    {
        var amendment = CreateOpenAmendment();
        amendment.Reject();
        amendment.UncommittedEvents.MarkAsCommitted();

        var action = () => amendment.Reject();

        action.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Only open amendments can be rejected*");
    }

    [Fact]
    public void Reject_WhenApproved_ShouldThrowBusinessRuleViolationException()
    {
        var amendment = CreateOpenAmendment();
        amendment.Accept();
        amendment.UncommittedEvents.MarkAsCommitted();

        var action = () => amendment.Reject();

        action.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Only open amendments can be rejected*");
    }

    [Fact]
    public void Reject_WhenWithdrawn_ShouldThrowBusinessRuleViolationException()
    {
        var amendment = CreateOpenAmendment();
        amendment.Withdraw(_creator);
        amendment.UncommittedEvents.MarkAsCommitted();

        var action = () => amendment.Reject();

        action.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Only open amendments can be rejected*");
    }

    [Fact]
    public void Reject_WhenOutdated_ShouldThrowBusinessRuleViolationException()
    {
        var amendment = CreateOpenAmendment();
        amendment.MarkOutdated(TargetVersionNumber + 1);
        amendment.UncommittedEvents.MarkAsCommitted();

        var action = () => amendment.Reject();

        action.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Only open amendments can be rejected*");
    }

    [Fact]
    public void Accept_WhenAlreadyRejected_ShouldThrowBusinessRuleViolationException()
    {
        var amendment = CreateOpenAmendment();
        amendment.Reject();
        amendment.UncommittedEvents.MarkAsCommitted();

        var action = () => amendment.Accept();

        action.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Only open amendments can be accepted*");
    }

    public class TheProposeMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();

        [Fact]
        public void GivenValidInputs_WhenProposed_ThenRaisesAmendmentProposedEvent()
        {
            var sut = Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentProposed);
        }

        [Fact]
        public void GivenEmptyProposedTitle_WhenProposed_ThenThrows()
        {
            var act = () => Amendment.Propose(_ideaId, 1, _creator, "", "Body", "Amendment title", "Amendment desc");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenVersionLessThanOne_WhenProposed_ThenThrows()
        {
            var act = () => Amendment.Propose(_ideaId, 0, _creator, "Title", "Body", "Amendment title", "Amendment desc");

            act.Should().Throw<ArgumentException>();
        }
    }

    public class TheAddSupportMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();
        private readonly ParticipantId _otherParticipant = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public void GivenOpenAmendment_WhenSupportAdded_ThenRaisesAmendmentSupportAddedEvent()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.AddSupport(new Supporter(_otherParticipant, "I agree"));

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentSupportAdded);
        }

        [Fact]
        public void GivenNonOpenAmendment_WhenSupportAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.Reject();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddSupport(new Supporter(_otherParticipant, "I agree"));

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenCreator_WhenSupportAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddSupport(new Supporter(_creator, "I agree"));

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenAlreadySupportingParticipant_WhenSupportAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.AddSupport(new Supporter(_otherParticipant, "First support"));
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddSupport(new Supporter(_otherParticipant, "Second support"));

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenAlreadyOpposingParticipant_WhenSupportAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.AddOpposition(new Opponent(_otherParticipant, "I oppose"));
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddSupport(new Supporter(_otherParticipant, "Actually I support"));

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheWithdrawSupportMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();
        private readonly ParticipantId _supporter = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public void GivenSupportingParticipant_WhenSupportWithdrawn_ThenRaisesAmendmentSupportWithdrawnEvent()
        {
            var sut = CreateOpenAmendment();
            sut.AddSupport(new Supporter(_supporter, "I agree"));
            sut.UncommittedEvents.MarkAsCommitted();

            sut.WithdrawSupport(_supporter);

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentSupportWithdrawn);
        }

        [Fact]
        public void GivenNonOpenAmendment_WhenSupportWithdrawn_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.Reject();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.WithdrawSupport(_supporter);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenParticipantWhoNeverSupported_WhenSupportWithdrawn_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.WithdrawSupport(_supporter);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheAddOppositionMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();
        private readonly ParticipantId _otherParticipant = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public void GivenOpenAmendment_WhenOppositionAdded_ThenRaisesAmendmentOppositionAddedEvent()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.AddOpposition(new Opponent(_otherParticipant, "I disagree"));

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentOppositionAdded);
        }

        [Fact]
        public void GivenNonOpenAmendment_WhenOppositionAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.Reject();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddOpposition(new Opponent(_otherParticipant, "I disagree"));

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenCreator_WhenOppositionAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddOpposition(new Opponent(_creator, "I disagree with myself"));

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenAlreadyOpposingParticipant_WhenOppositionAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.AddOpposition(new Opponent(_otherParticipant, "First opposition"));
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddOpposition(new Opponent(_otherParticipant, "Second opposition"));

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenAlreadySupportingParticipant_WhenOppositionAdded_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.AddSupport(new Supporter(_otherParticipant, "I support"));
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddOpposition(new Opponent(_otherParticipant, "Actually I oppose"));

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheWithdrawOppositionMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();
        private readonly ParticipantId _opponent = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public void GivenOpposingParticipant_WhenOppositionWithdrawn_ThenRaisesAmendmentOppositionWithdrawnEvent()
        {
            var sut = CreateOpenAmendment();
            sut.AddOpposition(new Opponent(_opponent, "I disagree"));
            sut.UncommittedEvents.MarkAsCommitted();

            sut.WithdrawOpposition(_opponent);

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentOppositionWithdrawn);
        }

        [Fact]
        public void GivenNonOpenAmendment_WhenOppositionWithdrawn_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.Reject();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.WithdrawOpposition(_opponent);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenParticipantWhoNeverOpposed_WhenOppositionWithdrawn_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.WithdrawOpposition(_opponent);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheAcceptMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public void GivenOpenAmendment_WhenAccepted_ThenRaisesAmendmentAcceptedEvent()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Accept();

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentAccepted);
        }

        [Fact]
        public void GivenOpenAmendment_WhenAccepted_ThenSetsStatusToApproved()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Accept();

            sut.Status.Should().Be(AmendmentStatus.Approved);
        }

        [Fact]
        public void GivenNonOpenAmendment_WhenAccepted_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.Reject();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Accept();

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheMarkOutdatedMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public void GivenOpenAmendment_WhenNewVersionIsHigher_ThenRaisesAmendmentMarkedOutdatedEvent()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.MarkOutdated(2);

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentMarkedOutdated);
        }

        [Fact]
        public void GivenOpenAmendment_WhenNewVersionIsHigher_ThenSetsStatusToOutdated()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.MarkOutdated(2);

            sut.Status.Should().Be(AmendmentStatus.Outdated);
        }

        [Fact]
        public void GivenOpenAmendment_WhenNewVersionIsNotHigher_ThenDoesNothing()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.MarkOutdated(1);

            sut.UncommittedEvents.Should().BeEmpty();
            sut.Status.Should().Be(AmendmentStatus.Open);
        }

        [Fact]
        public void GivenNonOpenAmendment_WhenMarkOutdated_ThenDoesNothing()
        {
            var sut = CreateOpenAmendment();
            sut.Reject();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.MarkOutdated(2);

            sut.UncommittedEvents.Should().BeEmpty();
            sut.Status.Should().Be(AmendmentStatus.Rejected);
        }
    }

    public class TheMarkOutdatedAfterApplicationFailureMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();

        private Amendment CreateApprovedAmendment()
        {
            var amendment = Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");
            amendment.Accept();
            amendment.UncommittedEvents.MarkAsCommitted();
            return amendment;
        }

        [Fact]
        public void GivenApprovedAmendment_WhenNewVersionIsHigher_ThenRaisesAmendmentMarkedOutdatedEvent()
        {
            var sut = CreateApprovedAmendment();

            sut.MarkOutdatedAfterApplicationFailure(2);

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentMarkedOutdated);
        }

        [Fact]
        public void GivenNonApprovedAmendment_WhenCalled_ThenThrows()
        {
            var sut = Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.MarkOutdatedAfterApplicationFailure(2);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheRetargetMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();

        private Amendment CreateOutdatedAmendment()
        {
            var amendment = Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");
            amendment.MarkOutdated(2);
            amendment.UncommittedEvents.MarkAsCommitted();
            return amendment;
        }

        [Fact]
        public void GivenOutdatedAmendmentAndCreator_WhenRetargeted_ThenRaisesAmendmentRetargetedEvent()
        {
            var sut = CreateOutdatedAmendment();

            sut.Retarget(_creator, 2);

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentRetargeted);
        }

        [Fact]
        public void GivenOutdatedAmendmentAndCreator_WhenRetargeted_ThenResetsStatusToOpen()
        {
            var sut = CreateOutdatedAmendment();

            sut.Retarget(_creator, 2);

            sut.Status.Should().Be(AmendmentStatus.Open);
        }

        [Fact]
        public void GivenNonOutdatedAmendment_WhenRetargeted_ThenThrows()
        {
            var sut = Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Retarget(_creator, 2);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenNonCreator_WhenRetargeted_ThenThrows()
        {
            var sut = CreateOutdatedAmendment();
            var otherParticipant = ParticipantId.Generate();

            var act = () => sut.Retarget(otherParticipant, 2);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenNewVersionNotGreaterThanCurrent_WhenRetargeted_ThenThrows()
        {
            var sut = CreateOutdatedAmendment();

            var act = () => sut.Retarget(_creator, 1);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenEmptyOptionalTitle_WhenRetargeted_ThenThrows()
        {
            var sut = CreateOutdatedAmendment();

            var act = () => sut.Retarget(_creator, 2, newTitle: "");

            act.Should().Throw<ArgumentException>();
        }
    }

    public class TheWithdrawMethod
    {
        private readonly IdeaId _ideaId = IdeaId.Generate();
        private readonly ParticipantId _creator = ParticipantId.Generate();

        private Amendment CreateOpenAmendment() =>
            Amendment.Propose(_ideaId, 1, _creator, "Title", "Body", "Amendment title", "Amendment desc");

        [Fact]
        public void GivenOpenAmendmentAndCreator_WhenWithdrawn_ThenRaisesAmendmentWithdrawnEvent()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Withdraw(_creator);

            sut.UncommittedEvents.Should().ContainSingle(e => e is AmendmentWithdrawn);
        }

        [Fact]
        public void GivenOpenAmendmentAndCreator_WhenWithdrawn_ThenSetsStatusToWithdrawn()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Withdraw(_creator);

            sut.Status.Should().Be(AmendmentStatus.Withdrawn);
        }

        [Fact]
        public void GivenNonOpenAmendment_WhenWithdrawn_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.Reject();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Withdraw(_creator);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenNonCreator_WhenWithdrawn_ThenThrows()
        {
            var sut = CreateOpenAmendment();
            sut.UncommittedEvents.MarkAsCommitted();
            var otherParticipant = ParticipantId.Generate();

            var act = () => sut.Withdraw(otherParticipant);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }
}
