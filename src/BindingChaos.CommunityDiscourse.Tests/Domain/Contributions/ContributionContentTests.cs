using BindingChaos.CommunityDiscourse.Domain.Contributions;
using FluentAssertions;

namespace BindingChaos.CommunityDiscourse.Tests.Domain.Contributions;

public class ContributionContentTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidContent_WhenCreated_ThenValueIsSet()
        {
            var sut = ContributionContent.Create("Hello, world!");

            sut.Value.Should().Be("Hello, world!");
        }

        [Fact]
        public void GivenContentWithLeadingAndTrailingWhitespace_WhenCreated_ThenValueIsTrimmed()
        {
            var sut = ContributionContent.Create("  Hello, world!  ");

            sut.Value.Should().Be("Hello, world!");
        }

        [Fact]
        public void GivenNullContent_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => ContributionContent.Create(null!);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenEmptyContent_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => ContributionContent.Create(string.Empty);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenWhitespaceOnlyContent_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => ContributionContent.Create("   ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenContentExceeding10000Characters_WhenCreated_ThenThrowsArgumentException()
        {
            var tooLongContent = new string('x', 10001);

            var act = () => ContributionContent.Create(tooLongContent);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenContentOfMaxLength_WhenCreated_ThenSucceeds()
        {
            var maxLengthContent = new string('x', 10000);

            var act = () => ContributionContent.Create(maxLengthContent);

            act.Should().NotThrow();
        }
    }

    public class TheEqualitySemantics
    {
        [Fact]
        public void GivenSameValue_WhenCompared_ThenAreEqual()
        {
            var first = ContributionContent.Create("Hello");
            var second = ContributionContent.Create("Hello");

            first.Should().Be(second);
        }

        [Fact]
        public void GivenDifferentValue_WhenCompared_ThenAreNotEqual()
        {
            var first = ContributionContent.Create("Hello");
            var second = ContributionContent.Create("World");

            first.Should().NotBe(second);
        }
    }

    public class TheTryCreateMethod
    {
        [Fact]
        public void GivenValidContent_WhenTryCreated_ThenReturnsInstance()
        {
            var sut = ContributionContent.TryCreate("Hello, world!");

            sut.Should().NotBeNull();
            sut!.Value.Should().Be("Hello, world!");
        }

        [Fact]
        public void GivenNullContent_WhenTryCreated_ThenReturnsNull()
        {
            var sut = ContributionContent.TryCreate(null);

            sut.Should().BeNull();
        }
    }
}
