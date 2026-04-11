using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Stigmergy.Domain.Concerns;
using BindingChaos.Stigmergy.Domain.Signals;
using Marten;
using Marten.Schema;

namespace BindingChaos.Stigmergy.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for the Concerns subdomain.
/// Seeds Concern aggregates linked to already-seeded signals.
/// Resolves signals by querying SignalCaptured events ordered by creation time.
/// Seed content is loaded from the embedded concerns-seed-data.json resource.
/// </summary>
public sealed class ConcernsInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcernsInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public ConcernsInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for Concerns.
    /// </summary>
    /// <param name="store">The Marten document store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
#pragma warning disable CA1725
    public async Task Populate(IDocumentStore store, CancellationToken cancellationToken)
#pragma warning restore CA1725
    {
        ArgumentNullException.ThrowIfNull(store);

#pragma warning disable CA2007
        await using IDocumentSession session = store.LightweightSession();
#pragma warning restore CA2007

        if (await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("ConcernRaised"))
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var signalIds = (await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("SignalCaptured") && e.StreamKey != null)
            .OrderBy(e => e.Timestamp)
            .Select(e => e.StreamKey!)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false))
            .Distinct()
            .ToList();

        if (signalIds.Count == 0)
        {
            return;
        }

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        var baseTime = DateTimeOffset.UtcNow.AddDays(-22);
        var timeProvider = new ControllableTimeProvider(baseTime);
        var originalTimeProvider = TimeProviderContext.Current;
        TimeProviderContext.SetCurrent(timeProvider);

        try
        {
            var random = new Random(42);

            foreach (var dto in data.Concerns)
            {
                timeProvider.AdvanceHours(random.Next(4, 20));

                var actor = _participants[dto.ActorIndex];

                var resolvedSignalIds = dto.SignalIndices
                    .Where(i => i < signalIds.Count)
                    .Select(i => SignalId.Create(signalIds[i]))
                    .ToList();

                if (resolvedSignalIds.Count == 0)
                {
                    continue;
                }

                var concern = Concern.Raise(
                    actor,
                    dto.Name,
                    dto.Tags,
                    resolvedSignalIds,
                    (ConcernOrigin)dto.Origin);

                foreach (var participantIndex in dto.AffectedParticipantIndices)
                {
                    if (participantIndex < _participants.Length)
                    {
                        timeProvider.AdvanceHours(random.Next(1, 12));
                        concern.IndicateAffectedness(_participants[participantIndex]);
                    }
                }

                session.Events.Append(concern.Id.Value, [.. concern.UncommittedEvents]);
            }

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<ConcernsSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(ConcernsInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Stigmergy.Infrastructure.Seeding.concerns-seed-data.json")
            ?? throw new InvalidOperationException("concerns-seed-data.json embedded resource not found in Stigmergy assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<ConcernsSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize concerns-seed-data.json.");
        }
    }

    private sealed class ConcernsSeedData
    {
        public List<ConcernDto> Concerns { get; set; } = [];
    }

    private sealed class ConcernDto
    {
        public int ActorIndex { get; set; }

        public string Name { get; set; } = string.Empty;

        public string[] Tags { get; set; } = [];

        public int[] SignalIndices { get; set; } = [];

        /// <summary>Gets or sets the ConcernOrigin enum value: 1 = Manual, 2 = EmergingPattern.</summary>
        public int Origin { get; set; }

        public int[] AffectedParticipantIndices { get; set; } = [];
    }
}
