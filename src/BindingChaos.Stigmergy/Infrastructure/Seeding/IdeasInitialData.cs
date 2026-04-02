using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Stigmergy.Domain.Ideas;
using Marten;
using Marten.Schema;

namespace BindingChaos.Stigmergy.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for Ideation using Marten's initial data hook.
/// Seeds idea and amendment events using aggregates to ensure domain logic integrity.
/// Depends on signals being seeded first via SignalAwarenessInitialData.
/// Seed content is loaded from the embedded idea-seed-data.json resource.
/// </summary>
public sealed class IdeasInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeasInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public IdeasInitialData(ParticipantId[] participants)
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

                var authorId = _participants[ideaDto.CreatorIndex];
                var idea = Idea.Draft(
                    authorId,
                    ideaDto.Title,
                    ideaDto.Body);

                idea.Publish(authorId);

                session.Events.Append(idea.Id.Value, [.. idea.UncommittedEvents]);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                createdIdeas.Add(idea.Id);
            }
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<IdeaSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(IdeasInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Stigmergy.Infrastructure.Seeding.idea-seed-data.json")
            ?? throw new InvalidOperationException("idea-seed-data.json embedded resource not found in Stigmergy assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<IdeaSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize Stigmergy idea-seed-data.json.");
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
    }
}
