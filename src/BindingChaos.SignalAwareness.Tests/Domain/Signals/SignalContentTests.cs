using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;

namespace BindingChaos.SignalAwareness.Tests.Domain.Signals;

public class SignalContentTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidTitleAndDescription_WhenCreated_ThenTrimsWhitespace()
        {
            var sut = SignalContent.Create("  Test Signal  ", "  A description  ");

            sut.Title.Should().Be("Test Signal");
            sut.Description.Should().Be("A description");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrEmptyTitle_WhenCreated_ThenThrowsArgumentException(string? title)
        {
            var act = () => SignalContent.Create(title!, "A valid description");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrEmptyDescription_WhenCreated_ThenThrowsArgumentException(string? description)
        {
            var act = () => SignalContent.Create("A valid title", description!);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenTitleExceeding200Characters_WhenCreated_ThenThrowsArgumentException()
        {
            var tooLongTitle = new string('a', 201);

            var act = () => SignalContent.Create(tooLongTitle, "A valid description");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenDescriptionExceeding2000Characters_WhenCreated_ThenThrowsArgumentException()
        {
            var tooLongDescription = new string('b', 2001);

            var act = () => SignalContent.Create("A valid title", tooLongDescription);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenTitleAtMaxLength_WhenCreated_ThenSucceeds()
        {
            var maxTitle = new string('a', 200);

            var sut = SignalContent.Create(maxTitle, "A valid description");

            sut.Title.Should().Be(maxTitle);
        }

        [Fact]
        public void GivenDescriptionAtMaxLength_WhenCreated_ThenSucceeds()
        {
            var maxDescription = new string('b', 2000);

            var sut = SignalContent.Create("A valid title", maxDescription);

            sut.Description.Should().Be(maxDescription);
        }
    }

    public class TheEqualitySemantics
    {
        [Fact]
        public void GivenSameTitleAndDescription_WhenCompared_ThenAreEqual()
        {
            var a = SignalContent.Create("Title", "Description");
            var b = SignalContent.Create("Title", "Description");

            a.Should().Be(b);
        }

        [Fact]
        public void GivenDifferentTitle_WhenCompared_ThenAreNotEqual()
        {
            var a = SignalContent.Create("Title One", "Description");
            var b = SignalContent.Create("Title Two", "Description");

            a.Should().NotBe(b);
        }
    }
}
