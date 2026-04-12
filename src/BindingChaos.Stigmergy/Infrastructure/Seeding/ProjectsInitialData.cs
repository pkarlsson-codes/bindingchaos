using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
using Marten.Schema;

namespace BindingChaos.Stigmergy.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for the Projects subdomain.
/// Seeds Project aggregates, one per entry in projects-seed-data.json.
/// Resolves user groups by querying UserGroupFormed events ordered by creation time.
/// Seed content is loaded from the embedded projects-seed-data.json resource.
/// </summary>
public sealed class ProjectsInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public ProjectsInitialData(ParticipantId[] participants)
    {
    }

    /// <summary>
    /// Populates initial development data for Projects.
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
            .Where(e => e.DotNetTypeName.Contains("ProjectCreated"))
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var userGroupIds = (await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("UserGroupFormed") && e.StreamKey != null)
            .OrderBy(e => e.Timestamp)
            .Select(e => e.StreamKey!)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false))
            .Distinct()
            .ToList();

        if (userGroupIds.Count == 0)
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

            foreach (var dto in data.Projects)
            {
                if (dto.UserGroupIndex >= userGroupIds.Count)
                {
                    continue;
                }

                timeProvider.AdvanceHours(random.Next(8, 48));

                var userGroupId = UserGroupId.Create(userGroupIds[dto.UserGroupIndex]);
                var project = Project.Create(userGroupId, dto.Title, dto.Description);
                session.Events.Append(project.Id.Value, [.. project.UncommittedEvents]);
            }

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<ProjectsSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(ProjectsInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Stigmergy.Infrastructure.Seeding.projects-seed-data.json")
            ?? throw new InvalidOperationException("projects-seed-data.json embedded resource not found in Stigmergy assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<ProjectsSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize projects-seed-data.json.");
        }
    }

    private sealed class ProjectsSeedData
    {
        public List<ProjectDto> Projects { get; set; } = [];
    }

    private sealed class ProjectDto
    {
        public int UserGroupIndex { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
