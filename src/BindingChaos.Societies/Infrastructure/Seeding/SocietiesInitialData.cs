using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Societies.Domain.SocialContracts;
using BindingChaos.Societies.Domain.Societies;
using Marten;
using Marten.Schema;

namespace BindingChaos.Societies.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for the Societies bounded context.
/// Seeds multiple societies with social contracts and participant memberships using domain aggregates.
/// Seed content is loaded from the embedded seed-data.json resource.
/// </summary>
public sealed class SocietiesInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="SocietiesInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public SocietiesInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for Societies.
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
            .Where(e => e.DotNetTypeName.Contains("SocietyCreated"))
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        var baseTime = DateTimeOffset.UtcNow.AddDays(-45);
        var timeProvider = new ControllableTimeProvider(baseTime);
        var originalTimeProvider = TimeProviderContext.Current;
        TimeProviderContext.SetCurrent(timeProvider);

        try
        {
            foreach (var entry in data.Societies)
            {
                timeProvider.AdvanceHours(8);

                var creator = _participants[entry.Society.CreatorIndex];

                var society = Society.Create(
                    creator,
                    entry.Society.Name,
                    entry.Society.Description,
                    entry.Society.Tags,
                    geographicBounds: null,
                    center: null);

                var contract = SocialContract.Establish(
                    society.Id,
                    creator,
                    new DecisionProtocol(
                        entry.SocialContract.RatificationThreshold,
                        TimeSpan.FromHours(entry.SocialContract.ReviewWindowHours),
                        entry.SocialContract.AllowVeto,
                        TimeSpan.FromHours(entry.SocialContract.InquiryLapseWindowHours)),
                    new EpistemicRules(entry.SocialContract.RequiredVerificationWeight));

                foreach (var index in entry.MemberIndices)
                {
                    if (index < _participants.Length)
                    {
                        timeProvider.AdvanceHours(1);
                        society.Join(_participants[index], contract.Id);
                    }
                }

                session.Events.Append(society.Id.Value, [.. society.UncommittedEvents]);
                session.Events.Append(contract.Id.Value, [.. contract.UncommittedEvents]);
            }

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<SocietySeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(SocietiesInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Societies.Infrastructure.Seeding.seed-data.json")
            ?? throw new InvalidOperationException("seed-data.json embedded resource not found in Societies assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<SocietySeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize Societies seed-data.json.");
        }
    }

    private sealed class SocietySeedData
    {
        public List<SocietyEntryDto> Societies { get; set; } = [];
    }

    private sealed class SocietyEntryDto
    {
        public SocietyDto Society { get; set; } = new();

        public SocialContractDto SocialContract { get; set; } = new();

        public int[] MemberIndices { get; set; } = [];
    }

    private sealed class SocietyDto
    {
        public int CreatorIndex { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string[] Tags { get; set; } = [];
    }

    private sealed class SocialContractDto
    {
        public double RatificationThreshold { get; set; }

        public double ReviewWindowHours { get; set; }

        public bool AllowVeto { get; set; }

        public double RequiredVerificationWeight { get; set; }

        public double InquiryLapseWindowHours { get; set; } = 336;
    }
}
