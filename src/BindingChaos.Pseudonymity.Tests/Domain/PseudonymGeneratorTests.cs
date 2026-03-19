using BindingChaos.Pseudonymity.Domain;

namespace BindingChaos.Pseudonymity.Tests.Domain;

public class PseudonymGeneratorTests
{
    private static bool IsValidPseudonym(string? pseudonym)
    {
        if (string.IsNullOrWhiteSpace(pseudonym)) return false;
        var parts = pseudonym.Split('-');
        return parts.Length == 3 && parts.All(IsValidWord);
    }

    private static bool IsValidWord(string? word)
    {
        if (string.IsNullOrWhiteSpace(word)) return false;
        return word.Length is >= 2 and <= 20 && word.All(char.IsLetter) && char.IsLower(word[0]);
    }


    public class TheGeneratePseudonymMethod
    {
        private const string SecretKey = "test-secret-key-for-testing";
        private const string AggregateType = "TestAggregate";
        private const string AggregateId = "test-aggregate-id";

        [Fact]
        public void GivenValidInputs_WhenCalled_ThenReturnsThreePartHyphenatedString()
        {
            var result = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, AggregateId, "user1");

            result.Split('-').Should().HaveCount(3);
        }

        [Fact]
        public void GivenValidInputs_WhenCalledTwiceWithSameArguments_ThenReturnsSamePseudonym()
        {
            const string userId = "user123";

            var first = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, AggregateId, userId);
            var second = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, AggregateId, userId);

            second.Should().Be(first);
        }

        [Fact]
        public void GivenSameAggregateAndDifferentUsers_WhenCalled_ThenReturnsDifferentPseudonyms()
        {
            var pseudonymForUser1 = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, AggregateId, "user-alpha");
            var pseudonymForUser2 = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, AggregateId, "user-beta");

            pseudonymForUser2.Should().NotBe(pseudonymForUser1);
        }

        [Fact]
        public void GivenSameUserAndDifferentAggregateIds_WhenCalled_ThenReturnsDifferentPseudonyms()
        {
            const string userId = "user1";

            var pseudonymForAgg1 = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, "aggregate-1", userId);
            var pseudonymForAgg2 = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, "aggregate-2", userId);

            pseudonymForAgg2.Should().NotBe(pseudonymForAgg1);
        }

        [Fact]
        public void GivenSameUserAndDifferentAggregateTypes_WhenCalled_ThenReturnsDifferentPseudonyms()
        {
            const string userId = "user1";

            var pseudonymForTypeA = PseudonymGenerator.GeneratePseudonym(SecretKey, "TypeAlpha", AggregateId, userId);
            var pseudonymForTypeB = PseudonymGenerator.GeneratePseudonym(SecretKey, "TypeBeta", AggregateId, userId);

            pseudonymForTypeB.Should().NotBe(pseudonymForTypeA);
        }

        [Fact]
        public void GivenSameInputAndDifferentSecretKeys_WhenCalled_ThenReturnsDifferentPseudonyms()
        {
            const string userId = "user1";

            var pseudonymWithKey1 = PseudonymGenerator.GeneratePseudonym("secret-key-1", AggregateType, AggregateId, userId);
            var pseudonymWithKey2 = PseudonymGenerator.GeneratePseudonym("secret-key-2", AggregateType, AggregateId, userId);

            pseudonymWithKey2.Should().NotBe(pseudonymWithKey1);
        }

        [Fact]
        public void GivenKnownInputs_WhenCalled_ThenReturnsExpectedPseudonym()
        {
            var result = PseudonymGenerator.GeneratePseudonym("pinned-secret-key-9f3k2m", "PinnedAggregate", "pinned-agg-001", "pinned-user-001");

            result.Should().Be("young-swan-care");
        }

        [Fact]
        public void GivenValidInputs_WhenCalled_ThenEachPartContainsOnlyLowercaseLetters()
        {
            var result = PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, AggregateId, "user1");
            var parts = result.Split('-');

            parts.Should().AllSatisfy(p => p.Should().MatchRegex(@"^[a-z]+$"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespaceSecretKey_WhenCalled_ThenThrows(string? secretKey)
        {
            var act = () => PseudonymGenerator.GeneratePseudonym(secretKey!, AggregateType, AggregateId, "user1");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespaceAggregateIdType_WhenCalled_ThenThrows(string? aggregateIdType)
        {
            var act = () => PseudonymGenerator.GeneratePseudonym(SecretKey, aggregateIdType!, AggregateId, "user1");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespaceAggregateIdValue_WhenCalled_ThenThrows(string? aggregateIdValue)
        {
            var act = () => PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, aggregateIdValue!, "user1");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespaceUserId_WhenCalled_ThenThrows(string? userId)
        {
            var act = () => PseudonymGenerator.GeneratePseudonym(SecretKey, AggregateType, AggregateId, userId!);

            act.Should().Throw<ArgumentException>();
        }
    }

    public class TheIsValidPseudonymMethod
    {
        [Theory]
        [InlineData("happy-dog-running", true)]
        [InlineData("happy", false)]
        [InlineData("Happy-Dog-Running", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void GivenInput_WhenValidated_ThenReturnsExpectedResult(string? input, bool expected)
        {
            var result = IsValidPseudonym(input);

            result.Should().Be(expected);
        }
    }

    public class TheIsValidWordMethod
    {
        [Theory]
        [InlineData("happy", true)]
        [InlineData("ab", true)]
        [InlineData("a", false)]
        [InlineData("Happy", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void GivenInput_WhenValidated_ThenReturnsExpectedResult(string? input, bool expected)
        {
            var result = IsValidWord(input);

            result.Should().Be(expected);
        }
    }
}
