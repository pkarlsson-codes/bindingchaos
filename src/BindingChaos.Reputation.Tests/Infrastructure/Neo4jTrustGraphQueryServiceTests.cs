using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.Reputation.Infrastructure.Persistence;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using FluentAssertions;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Tests.Infrastructure;

[Collection("Neo4j")]
public class Neo4jTrustGraphQueryServiceTests : IAsyncLifetime
{
    private readonly IDriver _driver;
    private readonly Neo4jTrustRelationshipRepository _repo;
    private readonly Neo4jTrustGraphQueryService _queryService;

    private static readonly ParticipantId Alice = new("participant-alice");
    private static readonly ParticipantId Bob = new("participant-bob");
    private static readonly ParticipantId Carol = new("participant-carol");
    private static readonly ParticipantId Dave = new("participant-dave");

    public Neo4jTrustGraphQueryServiceTests(Neo4jFixture fixture)
    {
        _driver = fixture.Driver;
        _repo = new Neo4jTrustRelationshipRepository(_driver);
        _queryService = new Neo4jTrustGraphQueryService(_driver);
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync("MATCH (n) DETACH DELETE n").ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task TrustAsync(ParticipantId truster, ParticipantId trustee)
    {
        TimeProviderContext.SetCurrent(new ControllableTimeProvider(DateTimeOffset.UtcNow));
        try
        {
            var rel = TrustRelationship.Create(truster, trustee);
            await _repo.TrustAsync(rel, CancellationToken.None);
        }
        finally
        {
            TimeProviderContext.Reset();
        }
    }

    [Fact]
    public async Task GetTrustedParticipants_WhenNoConnections_ReturnsEmptySet()
    {
        var result = await _queryService.GetTrustedParticipantsAsync(Alice, 1, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustedParticipants_DegreeOne_ReturnsDirectTrusteesOnly()
    {
        // Alice trusts Bob; Bob trusts Carol
        await TrustAsync(Alice, Bob);
        await TrustAsync(Bob, Carol);

        var result = await _queryService.GetTrustedParticipantsAsync(Alice, 1, CancellationToken.None);

        result.Should().ContainSingle().Which.Should().Be(Bob);
    }

    [Fact]
    public async Task GetTrustedParticipants_DegreeTwo_IncludesTrusteesOfTrustees()
    {
        // Alice trusts Bob; Bob trusts Carol
        await TrustAsync(Alice, Bob);
        await TrustAsync(Bob, Carol);

        var result = await _queryService.GetTrustedParticipantsAsync(Alice, 2, CancellationToken.None);

        result.Should().BeEquivalentTo(new[] { Bob, Carol });
    }

    [Fact]
    public async Task GetTrustedParticipants_DegreeClamped_DoesNotExceedMaxDegree()
    {
        // Alice-Bob-Carol-Dave (3 hops)
        await TrustAsync(Alice, Bob);
        await TrustAsync(Bob, Carol);
        await TrustAsync(Carol, Dave);

        var degreeTwo = await _queryService.GetTrustedParticipantsAsync(Alice, 2, CancellationToken.None);
        var degreeThree = await _queryService.GetTrustedParticipantsAsync(Alice, 3, CancellationToken.None);

        degreeTwo.Should().BeEquivalentTo(new[] { Bob, Carol });
        degreeTwo.Should().NotContain(Dave);

        degreeThree.Should().BeEquivalentTo(new[] { Bob, Carol, Dave });
    }

    [Fact]
    public async Task FilterTrustedParticipants_WhenNoneOfTheCandidatesAreTrusted_ReturnsEmptySet()
    {
        await TrustAsync(Alice, Bob);

        var result = await _queryService.FilterTrustedParticipantsAsync(Alice, 1, [Carol, Dave], CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FilterTrustedParticipants_ReturnsOnlyCandidatesThatAreTrusted()
    {
        // Alice trusts Bob and Carol, but not Dave
        await TrustAsync(Alice, Bob);
        await TrustAsync(Alice, Carol);

        var result = await _queryService.FilterTrustedParticipantsAsync(Alice, 1, [Bob, Carol, Dave], CancellationToken.None);

        result.Should().BeEquivalentTo(new[] { Bob, Carol });
    }

    [Fact]
    public async Task FilterTrustedParticipants_RespectsMaxDegree()
    {
        // Alice-Bob-Carol (2 hops); Dave is not in the chain
        await TrustAsync(Alice, Bob);
        await TrustAsync(Bob, Carol);

        var degreeOne = await _queryService.FilterTrustedParticipantsAsync(Alice, 1, [Bob, Carol], CancellationToken.None);
        var degreeTwo = await _queryService.FilterTrustedParticipantsAsync(Alice, 2, [Bob, Carol], CancellationToken.None);

        degreeOne.Should().ContainSingle().Which.Should().Be(Bob);
        degreeTwo.Should().BeEquivalentTo(new[] { Bob, Carol });
    }

    [Fact]
    public async Task FilterTrustedParticipants_WithEmptyCandidateList_ReturnsEmptySet()
    {
        await TrustAsync(Alice, Bob);

        var result = await _queryService.FilterTrustedParticipantsAsync(Alice, 1, [], CancellationToken.None);

        result.Should().BeEmpty();
    }
}
