using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;

namespace BindingChaos.SignalAwareness.Tests.Domain.Signals;

public class SignalIdTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidPrefixedValue_WhenCreated_ThenValueIsPreserved()
        {
            var value = "signal-01arz3ndektsv4rrffq69g5fav";

            var sut = SignalId.Create(value);

            sut.Value.Should().Be(value);
        }
    }

    public class TheGenerateMethod
    {
        [Fact]
        public void GivenNoInput_WhenGenerated_ThenProducesUlidWithSignalPrefix()
        {
            var sut = SignalId.Generate();

            sut.Value.Should().MatchRegex(@"^signal-[0-9a-hjkmnp-tv-z]{26}$");
        }

        [Fact]
        public void GivenMultipleCalls_WhenGenerated_ThenEachIdIsUnique()
        {
            var id1 = SignalId.Generate();
            var id2 = SignalId.Generate();

            id1.Should().NotBe(id2);
        }
    }
}
