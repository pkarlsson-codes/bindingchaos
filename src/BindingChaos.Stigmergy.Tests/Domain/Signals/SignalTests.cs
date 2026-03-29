using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Geography;
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
            string[] attachmentIds = ["attachmentIds"];

            var sut = Signal.Capture(
                actorId, "Signal title","Signal description.", tags, attachmentIds, new Coordinates(1, 2));

            var e = sut.UncommittedEvents.Should().ContainSingle().Which.Should().BeOfType<SignalCaptured>().Subject;
            e.CapturedById.Should().Be(actorId.Value);
            e.Title.Should().Be("Signal title");
            e.Description.Should().Be("Signal description.");
            e.Tags.Should().BeEquivalentTo(tags);
            e.AttachmentIds.Should().BeEquivalentTo(attachmentIds);
            e.Latitude.Should().Be(1);
            e.Longitude.Should().Be(2);
            sut.Id.Value.Should().StartWith("stigmergysignal");
        }

        [Fact]
        public void GivenNullActorId_WhenCaptured_ThenThrowsArgumentNullException()
        {
            string[] tags = ["urgency", "coordination"];
            string[] attachmentIds = ["attachmentIds"];
            
            var act = () => Signal.Capture(
                null, "Signal title","Signal description.", tags, attachmentIds, new Coordinates(1, 2));

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullOrWhiteSpaceDescription_WhenCaptured_ThenThrowsArgumentException()
        {
            var actorId = ParticipantId.Generate();
            string[] tags = ["urgency", "coordination"];
            string[] attachmentIds = ["attachmentIds"];
            
            var act = () => Signal.Capture(
                actorId, "Signal title"," ", tags, attachmentIds, new Coordinates(1, 2));

            act.Should().Throw<ArgumentException>();
        }
    }
}
