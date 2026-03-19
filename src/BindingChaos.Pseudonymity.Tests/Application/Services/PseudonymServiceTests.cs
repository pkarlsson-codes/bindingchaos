using BindingChaos.Pseudonymity.Application.Services;
using BindingChaos.Pseudonymity.Infrastructure.Configuration;
using BindingChaos.SharedKernel.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BindingChaos.Pseudonymity.Tests.Application.Services;

public class PseudonymServiceTests
{
    /// <summary>
    /// A concrete EntityId subtype used only in tests.
    /// </summary>
    private sealed class TestAggregateId : EntityId<TestAggregateId>
    {
        public const string Prefix = "testagg";

        public TestAggregateId(string value)
            : base(value, Prefix)
        {
        }
    }

    public class TestBed
    {
        public Mock<ILogger<PseudonymService>> Logger { get; } = new();
        public PseudonymService Sut { get; }

        public TestBed()
        {
            var options = Options.Create(new PseudonymityConfiguration { HmacSecretKey = "test-secret-key" });
            Sut = new PseudonymService(options, Logger.Object);
        }
    }

    public class TheConstructor
    {
        private readonly Mock<ILogger<PseudonymService>> _logger = new();

        [Fact]
        public void GivenEmptyHmacSecretKey_WhenConstructed_ThenThrowsArgumentException()
        {
            var options = Options.Create(new PseudonymityConfiguration { HmacSecretKey = string.Empty });

            var act = () => new PseudonymService(options, _logger.Object);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenWhitespaceHmacSecretKey_WhenConstructed_ThenThrowsArgumentException()
        {
            var options = Options.Create(new PseudonymityConfiguration { HmacSecretKey = "   " });

            var act = () => new PseudonymService(options, _logger.Object);

            act.Should().Throw<ArgumentException>();
        }
    }

    public class TheGenerateMethod
    {
        private readonly TestBed testBed = new();
        private static readonly TestAggregateId TestId = new("testagg-001");

        [Fact]
        public void GivenValidInputs_WhenGenerating_ThenReturnsThreePartHyphenatedString()
        {
            var result = testBed.Sut.Generate(TestId, "user-001");

            result.Split('-').Should().HaveCount(3);
        }

        [Fact]
        public void GivenSameInputsCalledTwice_WhenGenerating_ThenReturnsSamePseudonym()
        {
            var first = testBed.Sut.Generate(TestId, "user-determinism");
            var second = testBed.Sut.Generate(TestId, "user-determinism");

            second.Should().Be(first);
        }

        [Fact]
        public void GivenDifferentUserIds_WhenGenerating_ThenReturnsDifferentPseudonyms()
        {
            var pseudonymForAlpha = testBed.Sut.Generate(TestId, "user-alpha");
            var pseudonymForBeta = testBed.Sut.Generate(TestId, "user-beta");

            pseudonymForBeta.Should().NotBe(pseudonymForAlpha);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespaceUserId_WhenGenerating_ThenThrowsArgumentException(string? userId)
        {
            var act = () => testBed.Sut.Generate(TestId, userId!);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenNullAggregateId_WhenGenerating_ThenThrowsArgumentNullException()
        {
            var act = () => testBed.Sut.Generate((TestAggregateId)null!, "user-001");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenDifferentAggregateIds_WhenGenerating_ThenReturnsDifferentPseudonyms()
        {
            var idA = new TestAggregateId("testagg-aaa");
            var idB = new TestAggregateId("testagg-bbb");

            var pseudonymA = testBed.Sut.Generate(idA, "user-001");
            var pseudonymB = testBed.Sut.Generate(idB, "user-001");

            pseudonymB.Should().NotBe(pseudonymA);
        }
    }

    public class TheGenerateMultipleMethod
    {
        private readonly TestBed testBed = new();
        private static readonly TestAggregateId TestId = new("testagg-002");

        [Fact]
        public void GivenEmptyUserList_WhenGenerating_ThenReturnsEmptyDictionary()
        {
            var result = testBed.Sut.Generate(TestId, Array.Empty<string>());

            result.Should().BeEmpty();
        }

        [Fact]
        public void GivenSingleUserId_WhenGenerating_ThenReturnsDictionaryWithOneEntry()
        {
            var result = testBed.Sut.Generate(TestId, new[] { "user-single" });

            result.Should().ContainSingle(kvp => kvp.Key == "user-single");
        }

        [Fact]
        public void GivenDuplicateUserIds_WhenGenerating_ThenDeduplicatesAndReturnsOneEntryPerUniqueUser()
        {
            var userIds = new[] { "user-dup", "user-dup", "user-dup" };

            var result = testBed.Sut.Generate(TestId, userIds);

            result.Should().ContainSingle(kvp => kvp.Key == "user-dup");
        }

        [Fact]
        public void GivenNullUserIds_WhenGenerating_ThenThrowsArgumentNullException()
        {
            var act = () => testBed.Sut.Generate(TestId, (IEnumerable<string>)null!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullAggregateId_WhenGenerating_ThenThrowsArgumentNullException()
        {
            var act = () => testBed.Sut.Generate((TestAggregateId)null!, new[] { "user-001" });

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
