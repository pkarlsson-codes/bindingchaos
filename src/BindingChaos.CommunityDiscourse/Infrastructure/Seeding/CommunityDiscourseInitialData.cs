using System.Text.Json;
using BindingChaos.CommunityDiscourse.Domain.Contributions;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using Marten;
using Marten.Schema;

namespace BindingChaos.CommunityDiscourse.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for Community Discourse using Marten's initial data hook.
/// Seeds discourse events using aggregates to ensure domain logic integrity.
/// Depends on signals and ideas being seeded first.
/// Seed content is loaded from the embedded seed-data.json resource.
/// </summary>
public sealed class CommunityDiscourseInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunityDiscourseInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public CommunityDiscourseInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for Community Discourse.
    /// </summary>
    /// <param name="store">The Marten document store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
#pragma warning disable CA1725 // Parameter names should match base declaration
    public async Task Populate(IDocumentStore store, CancellationToken cancellationToken)
#pragma warning restore CA1725 // Parameter names should match base declaration
    {
        ArgumentNullException.ThrowIfNull(store);

#pragma warning disable CA2007
        await using var session = store.LightweightSession();
#pragma warning restore CA2007

        if (await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("DiscourseThreadCreated") || e.EventTypeName.Contains("ContributionPosted"))
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

        var ideaIds = await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("IdeaAuthored") && e.StreamKey != null)
            .Select(e => e.StreamKey!)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (signalIds.Count == 0 && ideaIds.Count == 0)
        {
            return;
        }

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        var baseTime = DateTimeOffset.UtcNow.AddDays(-20);
        var timeProvider = new ControllableTimeProvider(baseTime);
        var originalTimeProvider = TimeProviderContext.Current;
        TimeProviderContext.SetCurrent(timeProvider);

        try
        {
            var random = new Random(42);

            foreach (var threadDto in data.Threads)
            {
                var entityIds = threadDto.EntityType switch
                {
                    "Signal" => signalIds,
                    "Idea" => ideaIds,
                    _ => throw new InvalidOperationException($"Unknown entity type '{threadDto.EntityType}' in discourse seed data."),
                };

                if (threadDto.EntityIndex >= entityIds.Count)
                {
                    continue;
                }

                timeProvider.SetTime(DateTimeOffset.UtcNow.AddDays(-20).AddHours(random.Next(0, 24)));

                var entityId = entityIds[threadDto.EntityIndex];
                var thread = DiscourseThread.Create(EntityReference.Create(threadDto.EntityType, entityId));

                var contributions = new List<Contribution>();

                foreach (var contribDto in threadDto.Contributions)
                {
                    timeProvider.AdvanceHours(random.Next(1, 36));

                    ContributionId? parentId = null;
                    if (contribDto.ParentIndex.HasValue && contribDto.ParentIndex.Value < contributions.Count)
                    {
                        parentId = contributions[contribDto.ParentIndex.Value].Id;
                    }

                    contributions.Add(Contribution.Create(
                        thread.Id,
                        _participants[contribDto.AuthorIndex],
                        ContributionContent.Create(contribDto.Content),
                        parentId));
                }

                session.Events.Append(thread.Id.Value, [.. thread.UncommittedEvents]);
                foreach (var contribution in contributions)
                {
                    session.Events.Append(contribution.Id.Value, [.. contribution.UncommittedEvents]);
                }
            }

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<DiscourseSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(CommunityDiscourseInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.CommunityDiscourse.Infrastructure.Seeding.seed-data.json")
            ?? throw new InvalidOperationException("seed-data.json embedded resource not found in CommunityDiscourse assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<DiscourseSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize CommunityDiscourse seed-data.json.");
        }
    }

    private sealed class DiscourseSeedData
    {
        public List<ThreadDto> Threads { get; set; } = [];
    }

    private sealed class ThreadDto
    {
        public string EntityType { get; set; } = string.Empty;

        public int EntityIndex { get; set; }

        public List<ContributionDto> Contributions { get; set; } = [];
    }

    private sealed class ContributionDto
    {
        public int AuthorIndex { get; set; }

        public string Content { get; set; } = string.Empty;

        public int? ParentIndex { get; set; }
    }
}
