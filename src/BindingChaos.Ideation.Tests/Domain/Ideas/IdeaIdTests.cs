using BindingChaos.Ideation.Domain.Ideas;
using FluentAssertions;

namespace BindingChaos.Ideation.Tests.Domain.Ideas;

public class IdeaIdTests
{
    [Fact]
    public void Create_WithValidValue_ShouldNotThrow()
    {
        var signalValue = "idea-01arz3ndektsv4rrffq69g5fav";

        var signalId = IdeaId.Create(signalValue);

        signalId.Should().NotBeNull();
        signalId.Value.Should().BeEquivalentTo(signalValue);
    }

    [Fact]
    public void Generate_ShouldCreateValidUniqueSignalIds()
    {
        var signalId1 = IdeaId.Generate();
        var signalId2 = IdeaId.Generate();

        signalId1.Value.Should().MatchRegex(@"^idea-[0-9a-hjkmnp-tv-z]{26}$");
        signalId2.Value.Should().MatchRegex(@"^idea-[0-9a-hjkmnp-tv-z]{26}$");
        signalId1.Should().NotBe(signalId2);
    }
}