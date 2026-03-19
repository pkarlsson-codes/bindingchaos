using BindingChaos.Tagging.Domain.Tags;
using FluentAssertions;

namespace BindingChaos.Tagging.Tests.Domain.Tags;

public class TagIdTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidValue_WhenCreated_ThenValueIsPreserved()
        {
            var value = "tag-01arz3ndektsv4rrffq69g5fav";

            var id = TagId.Create(value);

            id.Value.Should().Be(value);
        }

        [Fact]
        public void GivenValueWithWrongPrefix_WhenCreated_ThenThrows()
        {
            var act = () => TagId.Create("signal-01arz3ndektsv4rrffq69g5fav");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenEmptyValue_WhenCreated_ThenThrows()
        {
            var act = () => TagId.Create(string.Empty);

            act.Should().Throw<ArgumentException>();
        }
    }

    public class TheGenerateMethod
    {
        [Fact]
        public void GivenNoInput_WhenGenerated_ThenMatchesExpectedFormat()
        {
            var id = TagId.Generate();

            id.Value.Should().MatchRegex(@"^tag-[0-9a-hjkmnp-tv-z]{26}$");
        }

        [Fact]
        public void GivenTwoCalls_WhenGenerated_ThenProduceDistinctIds()
        {
            var id1 = TagId.Generate();
            var id2 = TagId.Generate();

            id1.Should().NotBe(id2);
        }
    }
}
