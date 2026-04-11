using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Marten;
using Marten.Schema;

namespace BindingChaos.Stigmergy.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for the GoverningCommons subdomain.
/// Seeds Commons aggregates in the Proposed state. Activation occurs in UserGroupsInitialData
/// when the first user group is formed to govern each commons.
/// Seed content is loaded from the embedded commons-seed-data.json resource.
/// </summary>
public sealed class CommonsInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonsInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public CommonsInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for Commons.
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
            .Where(e => e.DotNetTypeName.Contains("CommonsCreated"))
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        var baseTime = DateTimeOffset.UtcNow.AddDays(-42);
        var timeProvider = new ControllableTimeProvider(baseTime);
        var originalTimeProvider = TimeProviderContext.Current;
        TimeProviderContext.SetCurrent(timeProvider);

        try
        {
            foreach (var dto in data.Commons)
            {
                timeProvider.AdvanceHours(4);

                var founder = _participants[dto.FounderIndex];
                var commons = Commons.Propose(dto.Name, dto.Description, founder);
                session.Events.Append(commons.Id.Value, [.. commons.UncommittedEvents]);
            }

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<CommonsSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(CommonsInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Stigmergy.Infrastructure.Seeding.commons-seed-data.json")
            ?? throw new InvalidOperationException("commons-seed-data.json embedded resource not found in Stigmergy assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<CommonsSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize commons-seed-data.json.");
        }
    }

    private sealed class CommonsSeedData
    {
        public List<CommonsDto> Commons { get; set; } = [];
    }

    private sealed class CommonsDto
    {
        public int FounderIndex { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
