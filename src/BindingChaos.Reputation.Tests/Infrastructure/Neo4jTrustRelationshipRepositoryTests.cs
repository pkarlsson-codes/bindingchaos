using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.Reputation.Infrastructure.Persistence;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using FluentAssertions;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Tests.Infrastructure;

[Collection("Neo4j")]
public class Neo4jTrustRelationshipRepositoryTests : IAsyncLifetime
{
    private readonly IDriver _driver;
    private readonly Neo4jTrustRelationshipRepository _repo;

    private static readonly ParticipantId Alice = new("participant-alice");
    private static readonly ParticipantId Bob = new("participant-bob");

    public Neo4jTrustRelationshipRepositoryTests(Neo4jFixture fixture)
    {
        _driver = fixture.Driver;
        _repo = new Neo4jTrustRelationshipRepository(_driver);
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

    [Fact]
    public async Task TrustAsync_ThenExistsAsync_ReturnsTrue()
    {
        TimeProviderContext.SetCurrent(new ControllableTimeProvider(DateTimeOffset.UtcNow));
        try
        {
            var relationship = TrustRelationship.Create(Alice, Bob);

            await _repo.TrustAsync(relationship, CancellationToken.None);
            var exists = await _repo.ExistsAsync(Alice, Bob, CancellationToken.None);

            exists.Should().BeTrue();
        }
        finally
        {
            TimeProviderContext.Reset();
        }
    }

    [Fact]
    public async Task TrustAsync_CalledTwice_IsIdempotent()
    {
        TimeProviderContext.SetCurrent(new ControllableTimeProvider(DateTimeOffset.UtcNow));
        try
        {
            var relationship = TrustRelationship.Create(Alice, Bob);

            await _repo.TrustAsync(relationship, CancellationToken.None);
            var act = () => _repo.TrustAsync(relationship, CancellationToken.None);

            await act.Should().NotThrowAsync();
            var exists = await _repo.ExistsAsync(Alice, Bob, CancellationToken.None);
            exists.Should().BeTrue();
        }
        finally
        {
            TimeProviderContext.Reset();
        }
    }

    [Fact]
    public async Task WithdrawAsync_ThenExistsAsync_ReturnsFalse()
    {
        TimeProviderContext.SetCurrent(new ControllableTimeProvider(DateTimeOffset.UtcNow));
        try
        {
            var relationship = TrustRelationship.Create(Alice, Bob);

            await _repo.TrustAsync(relationship, CancellationToken.None);
            await _repo.WithdrawAsync(Alice, Bob, CancellationToken.None);
            var exists = await _repo.ExistsAsync(Alice, Bob, CancellationToken.None);

            exists.Should().BeFalse();
        }
        finally
        {
            TimeProviderContext.Reset();
        }
    }

    [Fact]
    public async Task WithdrawAsync_WhenRelationshipDoesNotExist_IsNoOp()
    {
        var act = () => _repo.WithdrawAsync(Alice, Bob, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExistsAsync_WhenNoRelationship_ReturnsFalse()
    {
        var exists = await _repo.ExistsAsync(Alice, Bob, CancellationToken.None);

        exists.Should().BeFalse();
    }
}
