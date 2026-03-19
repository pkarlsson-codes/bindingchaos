using BindingChaos.Pseudonymity.Domain;
using BindingChaos.Pseudonymity.Infrastructure.Configuration;
using BindingChaos.SharedKernel.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BindingChaos.Pseudonymity.Application.Services;

/// <summary>
/// Provides deterministic pseudonym generation using HMAC-SHA256.
/// </summary>
public sealed class PseudonymService : IPseudonymService
{
    private static readonly Action<ILogger, int, string, string, Exception?> LogGeneratingPseudonyms =
        LoggerMessage.Define<int, string, string>(
            LogLevel.Debug,
            new EventId(1, nameof(LogGeneratingPseudonyms)),
            "Generating {Count} pseudonyms for aggregate {AggregateType}:{AggregateId}");

    private readonly string _secretKey;
    private readonly ILogger<PseudonymService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PseudonymService"/> class.
    /// </summary>
    public PseudonymService(
        IOptions<PseudonymityConfiguration> configuration,
        ILogger<PseudonymService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _secretKey = configuration.Value.HmacSecretKey;
        ArgumentException.ThrowIfNullOrWhiteSpace(_secretKey, nameof(configuration));
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Generate<TAggregateId>(
        TAggregateId aggregateId,
        IEnumerable<string> userIds)
        where TAggregateId : EntityId
    {
        ArgumentNullException.ThrowIfNull(aggregateId);
        var userIdList = userIds?.Distinct().ToList() ?? throw new ArgumentNullException(nameof(userIds));

        if (userIdList.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        var aggregateIdType = typeof(TAggregateId).Name;
        var aggregateIdValue = aggregateId.Value;

        LogGeneratingPseudonyms(_logger, userIdList.Count, aggregateIdType, aggregateIdValue, null);

        var resultMap = new Dictionary<string, string>(userIdList.Count);

        foreach (var userId in userIdList)
        {
            var pseudonym = PseudonymGenerator.GeneratePseudonym(_secretKey, aggregateIdType, aggregateIdValue, userId);
            resultMap[userId] = pseudonym;
        }

        return resultMap;
    }

    /// <inheritdoc />
    public string Generate<TAggregateId>(
        TAggregateId aggregateId,
        string userId)
        where TAggregateId : EntityId
    {
        ArgumentNullException.ThrowIfNull(aggregateId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var aggregateIdType = typeof(TAggregateId).Name;
        var aggregateIdValue = aggregateId.Value;

        return PseudonymGenerator.GeneratePseudonym(_secretKey, aggregateIdType, aggregateIdValue, userId);
    }
}
