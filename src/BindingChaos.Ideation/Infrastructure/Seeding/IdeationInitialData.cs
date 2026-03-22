using System.Text.Json;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using Marten;
using Marten.Schema;

namespace BindingChaos.Ideation.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for Ideation using Marten's initial data hook.
/// Seeds idea and amendment events using aggregates to ensure domain logic integrity.
/// Depends on signals being seeded first via SignalAwarenessInitialData.
/// Seed content is loaded from the embedded seed-data.json resource.
/// </summary>
public sealed class IdeationInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeationInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public IdeationInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for Ideation.
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
            .Where(e => e.DotNetTypeName.Contains("IdeaAuthored"))
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var signalIds = await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("SignalCaptured") && e.StreamKey != null)
            .Select(e => e.StreamKey!)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (signalIds.Count == 0)
        {
            return;
        }

        var societyIds = await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("SocietyCreated") && e.StreamKey != null)
            .Select(e => e.StreamKey!)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (societyIds.Count == 0)
        {
            return;
        }

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        var baseTime = DateTimeOffset.UtcNow.AddDays(-25);
        var timeProvider = new ControllableTimeProvider(baseTime);
        var originalTimeProvider = TimeProviderContext.Current;
        TimeProviderContext.SetCurrent(timeProvider);

        try
        {
            var random = new Random(42);
            var createdIdeas = new List<IdeaId>();

            foreach (var ideaDto in data.Ideas)
            {
                timeProvider.AdvanceHours(random.Next(6, 18));

                var referencedSignals = ideaDto.SignalIndices
                    .Where(i => i < signalIds.Count)
                    .Select(i => signalIds[i])
                    .ToArray();

                var idea = Idea.Author(
                    SocietyId.Create(societyIds[0]),
                    _participants[ideaDto.CreatorIndex],
                    ideaDto.Title,
                    ideaDto.Body,
                    referencedSignals,
                    ideaDto.Tags);

                session.Events.Append(idea.Id.Value, idea.UncommittedEvents.ToArray());
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                createdIdeas.Add(idea.Id);
            }

            for (var i = 0; i < data.Ideas.Count; i++)
            {
                await CreateAmendmentsForIdea(
                    session, createdIdeas[i], data.Ideas[i].Amendments, random, timeProvider, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<IdeaSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(IdeationInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Ideation.Infrastructure.Seeding.seed-data.json")
            ?? throw new InvalidOperationException("seed-data.json embedded resource not found in Ideation assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<IdeaSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize Ideation seed-data.json.");
        }
    }

    private async Task CreateAmendmentsForIdea(
        IDocumentSession session,
        IdeaId ideaId,
        List<AmendmentDto> amendments,
        Random random,
        ControllableTimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        foreach (var dto in amendments)
        {
            timeProvider.AdvanceHours(random.Next(12, 48));

            var amendment = Amendment.Propose(
                ideaId,
                1,
                _participants[dto.CreatorIndex],
                dto.ProposedTitle,
                dto.ProposedBody,
                dto.AmendmentTitle,
                dto.AmendmentDescription);

            foreach (var supporter in dto.Supporters)
            {
                timeProvider.AdvanceHours(random.Next(2, 18));
                amendment.AddSupport(new Supporter(_participants[supporter.ParticipantIndex], supporter.Reason));
            }

            foreach (var opponent in dto.Opponents)
            {
                timeProvider.AdvanceHours(random.Next(2, 18));
                amendment.AddOpposition(new Opponent(_participants[opponent.ParticipantIndex], opponent.Reason));
            }

            session.Events.Append(amendment.Id.Value, amendment.UncommittedEvents.ToArray());
            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private sealed class IdeaSeedData
    {
        public List<IdeaDto> Ideas { get; set; } = [];
    }

    private sealed class IdeaDto
    {
        public int CreatorIndex { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public int[] SignalIndices { get; set; } = [];

        public string[] Tags { get; set; } = [];

        public List<AmendmentDto> Amendments { get; set; } = [];
    }

    private sealed class AmendmentDto
    {
        public int CreatorIndex { get; set; }

        public string ProposedTitle { get; set; } = string.Empty;

        public string ProposedBody { get; set; } = string.Empty;

        public string AmendmentTitle { get; set; } = string.Empty;

        public string AmendmentDescription { get; set; } = string.Empty;

        public List<VoterDto> Supporters { get; set; } = [];

        public List<VoterDto> Opponents { get; set; } = [];
    }

    private sealed class VoterDto
    {
        public int ParticipantIndex { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
