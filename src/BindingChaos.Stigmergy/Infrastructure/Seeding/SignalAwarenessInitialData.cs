using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Stigmergy.Domain.Signals;
using Marten;
using Marten.Schema;

namespace BindingChaos.Stigmergy.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for Stigmergy using Marten's initial data hook.
/// Seeds signal events using aggregates to ensure domain logic integrity.
/// Seed content is loaded from the embedded seed-data.json resource.
/// </summary>
public sealed class StigmergyInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="StigmergyInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public StigmergyInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for Stigmergy.
    /// </summary>
    /// <param name="store">The Marten document store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
#pragma warning disable CA1725 // Parameter names should match base declaration
    public async Task Populate(IDocumentStore store, CancellationToken cancellationToken)
#pragma warning restore CA1725
    {
        ArgumentNullException.ThrowIfNull(store);

#pragma warning disable CA2007
        await using IDocumentSession session = store.LightweightSession();
#pragma warning restore CA2007

        if (await session.Events.QueryAllRawEvents()
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        var baseTime = DateTimeOffset.UtcNow.AddDays(-30);
        var timeProvider = new ControllableTimeProvider(baseTime);
        var originalTimeProvider = TimeProviderContext.Current;
        TimeProviderContext.SetCurrent(timeProvider);

        try
        {
            var random = new Random(42);

            foreach (var signalDto in data.Signals)
            {
                timeProvider.AdvanceHours(random.Next(2, 16));

                var signal = Signal.Capture(
                    _participants[signalDto.OriginatorIndex],
                    signalDto.Title,
                    signalDto.Description,
                    signalDto.Tags,
                    [],
                    null);

                foreach (var amp in signalDto.Amplifications)
                {
                    timeProvider.AdvanceHours(random.Next(1, 24));
                    signal.Amplify(_participants[amp.ParticipantIndex]);
                }

                session.Events.Append(signal.Id.Value, [.. signal.UncommittedEvents]);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<SignalSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(StigmergyInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Stigmergy.Infrastructure.Seeding.seed-data.json")
            ?? throw new InvalidOperationException("seed-data.json embedded resource not found in SignalAwareness assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<SignalSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize SignalAwareness seed-data.json.");
        }
    }

    private sealed class SignalSeedData
    {
        public List<SignalDto> Signals { get; set; } = [];
    }

    private sealed class SignalDto
    {
        public int OriginatorIndex { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string[] Tags { get; set; } = [];

        public List<AmplificationDto> Amplifications { get; set; } = [];

        public List<SuggestedActionDto> SuggestedActions { get; set; } = [];

        public List<EvidenceDto> Evidence { get; set; } = [];
    }

    private sealed class AmplificationDto
    {
        public int ParticipantIndex { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;
    }

    private sealed class SuggestedActionDto
    {
        /// <summary>Gets or sets the action type: "MakeACall" or "VisitAWebpage".</summary>
        public string Type { get; set; } = string.Empty;

        public int ParticipantIndex { get; set; }

        /// <summary>Gets or sets the phone number. Required when Type is "MakeACall".</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Gets or sets the URL. Required when Type is "VisitAWebpage".</summary>
        public string? Url { get; set; }

        public string? Details { get; set; }
    }

    private sealed class EvidenceDto
    {
        public int ParticipantIndex { get; set; }

        public string Description { get; set; } = string.Empty;

        public string[] DocumentIds { get; set; } = [];
    }
}
