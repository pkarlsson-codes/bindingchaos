using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Concerns;
using BindingChaos.Stigmergy.Domain.Concerns.Events;
using BindingChaos.Stigmergy.Domain.Signals;
using FluentAssertions;

namespace BindingChaos.Stigmergy.Tests.Domain.Concerns;

public class ConcernTests
{
    public class TheRaiseMethod
    {
        private static SignalId[] SomeSignalIds() => [SignalId.Generate(), SignalId.Generate()];
        private static string[] SomeTags() => ["housing", "shortage"];

        [Fact]
        public void GivenValidInputs_WhenRaised_ThenRaisesConcernRaisedEvent()
        {
            var actorId = ParticipantId.Generate();
            var tags = SomeTags();
            var signalIds = SomeSignalIds();

            var sut = Concern.Raise(actorId, "Coordination failure", tags, signalIds, ConcernOrigin.Manual);

            var e = sut.UncommittedEvents.Should().ContainSingle().Which.Should().BeOfType<ConcernRaised>().Subject;
            e.ActorId.Should().Be(actorId.Value);
            e.Name.Should().Be("Coordination failure");
            e.SignalIds.Should().BeEquivalentTo(signalIds.Select(s => s.Value));
            sut.Id.Value.Should().StartWith("concern");
        }

        [Fact]
        public void GivenNullActorId_WhenRaised_ThenThrowsArgumentNullException()
        {
            var act = () => Concern.Raise(null!, "Name", SomeTags(), SomeSignalIds(), ConcernOrigin.Manual);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullOrWhiteSpaceName_WhenRaised_ThenThrowsArgumentException()
        {
            var act = () => Concern.Raise(ParticipantId.Generate(), "  ", SomeTags(), SomeSignalIds(), ConcernOrigin.Manual);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenNoSignalIds_WhenRaised_ThenThrowsBusinessRuleViolationException()
        {
            var act = () => Concern.Raise(ParticipantId.Generate(), "Name", SomeTags(), [], ConcernOrigin.Manual);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }
}
