using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Domain.Signals;
using BindingChaos.Stigmergy.Domain.Signals.Events;
using FluentAssertions;

namespace BindingChaos.Stigmergy.Tests.Domain.Signals;

public class SignalTests
{
    public class TheCaptureMethod
    {
        [Fact]
        public void GivenValidInputs_WhenCaptured_ThenRaisesSignalCapturedEvent()
        {
            var actorId = ParticipantId.Generate();
            string[] tags = ["urgency", "coordination"];

            var sut = Signal.Capture(actorId, "A weak signal", tags);

            var e = sut.UncommittedEvents.Should().ContainSingle().Which.Should().BeOfType<SignalCaptured>().Subject;
            e.CapturedById.Should().Be(actorId.Value);
            e.Description.Should().Be("A weak signal");
            e.Tags.Should().BeEquivalentTo(tags);
            sut.Id.Value.Should().StartWith("stigmergysignal");
        }

        [Fact]
        public void GivenNullActorId_WhenCaptured_ThenThrowsArgumentNullException()
        {
            var act = () => Signal.Capture(null!, "A weak signal", []);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullOrWhiteSpaceDescription_WhenCaptured_ThenThrowsArgumentException()
        {
            var act = () => Signal.Capture(ParticipantId.Generate(), "  ", []);

            act.Should().Throw<ArgumentException>();
        }
    }
}
