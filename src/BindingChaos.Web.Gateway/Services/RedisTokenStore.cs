using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Redis-backed token store suitable for multi-instance BFF. Stores tokens per session key with TTL.
/// </summary>
public sealed class RedisTokenStore : ITokenStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IDatabase _db;
    private readonly TimeSpan _defaultTtl;

    public RedisTokenStore(IConnectionMultiplexer connection, IConfiguration configuration)
    {
        _db = connection.GetDatabase();
        var minutes = configuration.GetValue<int?>("Authentication:TokenStore:DefaultTtlMinutes") ?? 60 * 24 * 7; // 7 days
        _defaultTtl = TimeSpan.FromMinutes(minutes);
    }

    /// <inheritdoc/>
    public async Task SaveTokensAsync(string sessionId, string userId, string? accessToken, string? refreshToken, string? idToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var key = BuildKey(sessionId);
        var model = new TokenRecord(userId, accessToken, refreshToken, idToken, DateTimeOffset.UtcNow);
        var json = JsonSerializer.Serialize(model, SerializerOptions);
        await _db.StringSetAsync(key, json, _defaultTtl, When.Always, CommandFlags.DemandMaster).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<(string userId, string? accessToken, string? refreshToken, string? idToken)?> TryGetTokensAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

        var key = BuildKey(sessionId);
        var value = await _db.StringGetAsync(key).ConfigureAwait(false);
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        var record = JsonSerializer.Deserialize<TokenRecord>(value.ToString(), SerializerOptions);
        if (record is null)
        {
            return null;
        }

        return (record.UserId, record.AccessToken, record.RefreshToken, record.IdToken);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

        var key = BuildKey(sessionId);
        await _db.KeyDeleteAsync(key).ConfigureAwait(false);
    }

    private static string BuildKey(string sessionId) => $"bc:sessions:{sessionId}";

    private sealed class TokenRecord
    {
        public TokenRecord(string userId, string? accessToken, string? refreshToken, string? idToken, DateTimeOffset createdAtUtc)
        {
            UserId = userId;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            IdToken = idToken;
            CreatedAtUtc = createdAtUtc;
        }

        public string UserId { get; }

        public string? AccessToken { get; }

        public string? RefreshToken { get; }

        public string? IdToken { get; }

        public DateTimeOffset CreatedAtUtc { get; }
    }
}
