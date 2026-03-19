using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.Signals.Events;
using FluentAssertions;

namespace BindingChaos.SignalAwareness.Tests.Domain.Signals;

public class SignalTests
{
    public class TheCaptureMethod
    {
        [Fact]
        public void GivenValidInputs_WhenCaptured_ThenRaisesSignalCapturedEvent()
        {
            var sut = Signal.Capture(SignalContent.Create("Title", "Description"),
                ParticipantId.Generate(), null, [], []);

            sut.UncommittedEvents.Should().ContainSingle(e => e is SignalCaptured);
        }

        [Fact]
        public void GivenValidInputs_WhenCaptured_ThenStatusIsActive()
        {
            var sut = Signal.Capture(SignalContent.Create("Title", "Description"),
                ParticipantId.Generate(), null, [], []);

            sut.Status.Should().Be(SignalStatus.Active);
        }

        [Fact]
        public void GivenValidInputs_WhenCaptured_ThenOriginatorIsSet()
        {
            var originator = ParticipantId.Generate();

            var sut = Signal.Capture(SignalContent.Create("Title", "Description"),
                originator, null, [], []);

            sut.OriginatorId.Should().Be(originator);
        }

        [Fact]
        public void GivenNullContent_WhenCaptured_ThenThrowsArgumentNullException()
        {
            var act = () => Signal.Capture(null!, ParticipantId.Generate(), null, [], []);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullOriginatorId_WhenCaptured_ThenThrowsArgumentNullException()
        {
            var act = () => Signal.Capture(SignalContent.Create("Title", "Description"),
                null!, null, [], []);

            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class TheAmplifyMethod
    {
        private static Signal CreateActiveSignal() =>
            Signal.Capture(SignalContent.Create("Test Signal", "A description"),
                ParticipantId.Generate(), null, [], []);

        [Fact]
        public void GivenActiveSignalAndNewParticipant_WhenAmplified_ThenRaisesSignalAmplifiedEvent()
        {
            var sut = CreateActiveSignal();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Amplify(ParticipantId.Generate(), AmplificationReason.HighRelevance);

            sut.UncommittedEvents.Should().ContainSingle(e => e is SignalAmplified);
        }

        [Fact]
        public void GivenActiveSignalAndNewParticipant_WhenAmplified_ThenAmplificationIsActive()
        {
            var sut = CreateActiveSignal();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Amplify(ParticipantId.Generate(), AmplificationReason.HighRelevance);

            sut.ActiveAmplifications.Should().HaveCount(1);
        }

        [Fact]
        public void GivenOriginator_WhenAmplified_ThenThrowsBusinessRuleViolationException()
        {
            var originator = ParticipantId.Generate();
            var sut = Signal.Capture(SignalContent.Create("Title", "Description"),
                originator, null, [], []);
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Amplify(originator, AmplificationReason.HighRelevance);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenParticipantAlreadyAmplified_WhenAmplifiedAgain_ThenThrowsBusinessRuleViolationException()
        {
            var amplifier = ParticipantId.Generate();
            var sut = CreateActiveSignal();
            sut.Amplify(amplifier, AmplificationReason.HighRelevance);
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Amplify(amplifier, AmplificationReason.PersonalExperience);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenNullAmplifierId_WhenAmplified_ThenThrowsArgumentNullException()
        {
            var sut = CreateActiveSignal();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Amplify(null!, AmplificationReason.HighRelevance);

            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class TheAttenuateMethod
    {
        private static Signal CreateActiveSignal() =>
            Signal.Capture(SignalContent.Create("Test Signal", "A description"),
                ParticipantId.Generate(), null, [], []);

        [Fact]
        public void GivenActiveSignalWithAmplification_WhenAttenuated_ThenRaisesSignalAttenuatedEvent()
        {
            var amplifier = ParticipantId.Generate();
            var sut = CreateActiveSignal();
            sut.Amplify(amplifier, AmplificationReason.HighRelevance);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Attenuate(amplifier);

            sut.UncommittedEvents.Should().ContainSingle(e => e is SignalAmplificationAttenuated);
        }

        [Fact]
        public void GivenActiveSignalWithAmplification_WhenAttenuated_ThenAmplificationIsNoLongerActive()
        {
            var amplifier = ParticipantId.Generate();
            var sut = CreateActiveSignal();
            sut.Amplify(amplifier, AmplificationReason.HighRelevance);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Attenuate(amplifier);

            sut.ActiveAmplifications.Should().BeEmpty();
        }

        [Fact]
        public void GivenParticipantWithNoAmplification_WhenAttenuated_ThenThrowsBusinessRuleViolationException()
        {
            var sut = CreateActiveSignal();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Attenuate(ParticipantId.Generate());

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheUpdateContentMethod
    {
        private static Signal CreateActiveSignal() =>
            Signal.Capture(SignalContent.Create("Test Signal", "A description"),
                ParticipantId.Generate(), null, [], []);

        [Fact]
        public void GivenActiveSignal_WhenContentUpdated_ThenRaisesSignalContentUpdatedEvent()
        {
            var sut = CreateActiveSignal();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.UpdateContent(SignalContent.Create("New Title", "New description"));

            sut.UncommittedEvents.Should().ContainSingle(e => e is SignalContentUpdated);
        }

        [Fact]
        public void GivenActiveSignal_WhenContentUpdated_ThenContentChanges()
        {
            var sut = CreateActiveSignal();
            sut.UncommittedEvents.MarkAsCommitted();
            var newContent = SignalContent.Create("New Title", "New description");

            sut.UpdateContent(newContent);

            sut.Content.Should().Be(newContent);
        }

        [Fact]
        public void GivenNullContent_WhenContentUpdated_ThenThrowsArgumentNullException()
        {
            var sut = CreateActiveSignal();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.UpdateContent(null!);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
