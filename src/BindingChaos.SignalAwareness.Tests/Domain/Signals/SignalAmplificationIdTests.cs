using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;

namespace BindingChaos.SignalAwareness.Tests.Domain.Signals;

public class SignalAmplificationIdTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidPrefixedValue_WhenCreated_ThenValueIsPreserved()
        {
            var value = "amplification-01arz3ndektsv4rrffq69g5fav";

            var sut = SignalAmplificationId.Create(value);

            sut.Value.Should().Be(value);
        }
    }

    public class TheGenerateMethod
    {
        [Fact]
        public void GivenNoInput_WhenGenerated_ThenProducesUlidWithCorrectPrefix()
        {
            var sut = SignalAmplificationId.Generate();

            sut.Value.Should().MatchRegex(@"^amplification-[0-9a-hjkmnp-tv-z]{26}$");
        }

        [Fact]
        public void GivenMultipleCalls_WhenGenerated_ThenEachIdIsUnique()
        {
            var id1 = SignalAmplificationId.Generate();
            var id2 = SignalAmplificationId.Generate();

            id1.Should().NotBe(id2);
        }
    }
}
