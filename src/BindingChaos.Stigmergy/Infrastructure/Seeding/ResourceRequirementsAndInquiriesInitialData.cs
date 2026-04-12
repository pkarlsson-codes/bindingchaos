using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.ResourceRequirements;
using Marten;
using Marten.Schema;

namespace BindingChaos.Stigmergy.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for ResourceRequirements and ProjectInquiries.
/// Seeds resource requirements (with pledges) and project inquiries (some with responses)
/// against already-seeded projects and societies.
/// Resolves projects by querying ProjectCreated events and societies by querying SocietyCreated events,
/// both ordered by creation time.
/// Seed content is loaded from the embedded requirements-and-inquiries-seed-data.json resource.
/// </summary>
public sealed class ResourceRequirementsAndInquiriesInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceRequirementsAndInquiriesInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public ResourceRequirementsAndInquiriesInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for ResourceRequirements and ProjectInquiries.
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
            .Where(e => e.DotNetTypeName.Contains("RequirementAdded"))
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var projectIds = (await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("ProjectCreated") && e.StreamKey != null)
            .OrderBy(e => e.Timestamp)
            .Select(e => e.StreamKey!)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false))
            .Distinct()
            .ToList();

        var societyIds = (await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("SocietyCreated") && e.StreamKey != null)
            .OrderBy(e => e.Timestamp)
            .Select(e => e.StreamKey!)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false))
            .Distinct()
            .ToList();

        if (projectIds.Count == 0)
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

            foreach (var dto in data.Requirements)
            {
                if (dto.ProjectIndex >= projectIds.Count)
                {
                    continue;
                }

                timeProvider.AdvanceHours(random.Next(4, 24));

                var projectId = ProjectId.Create(projectIds[dto.ProjectIndex]);
                var requirement = ResourceRequirement.Create(projectId, dto.Description, dto.QuantityNeeded, dto.Unit);

                foreach (var pledge in dto.Pledges)
                {
                    if (pledge.ParticipantIndex < _participants.Length)
                    {
                        timeProvider.AdvanceHours(random.Next(1, 12));
                        requirement.PledgeResources(_participants[pledge.ParticipantIndex], pledge.Amount);
                    }
                }

                session.Events.Append(requirement.Id.Value, [.. requirement.UncommittedEvents]);
            }

            foreach (var dto in data.Inquiries)
            {
                if (dto.ProjectIndex >= projectIds.Count || dto.SocietyIndex >= societyIds.Count)
                {
                    continue;
                }

                if (dto.RaisedByParticipantIndex >= _participants.Length)
                {
                    continue;
                }

                timeProvider.AdvanceHours(random.Next(12, 72));

                var projectId = ProjectId.Create(projectIds[dto.ProjectIndex]);
                var raiser = _participants[dto.RaisedByParticipantIndex];
                var societyId = societyIds[dto.SocietyIndex];

                var inquiry = ProjectInquiry.Raise(
                    projectId,
                    raiser,
                    societyId,
                    dto.Body,
                    TimeSpan.FromHours(dto.LapseWindowHours));

                if (!string.IsNullOrWhiteSpace(dto.Response))
                {
                    timeProvider.AdvanceHours(random.Next(24, 96));
                    inquiry.Respond(dto.Response);
                }

                session.Events.Append(inquiry.Id.Value, [.. inquiry.UncommittedEvents]);
            }

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<SeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(ResourceRequirementsAndInquiriesInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Stigmergy.Infrastructure.Seeding.requirements-and-inquiries-seed-data.json")
            ?? throw new InvalidOperationException("requirements-and-inquiries-seed-data.json embedded resource not found in Stigmergy assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<SeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize requirements-and-inquiries-seed-data.json.");
        }
    }

    private sealed class SeedData
    {
        public List<RequirementDto> Requirements { get; set; } = [];

        public List<InquiryDto> Inquiries { get; set; } = [];
    }

    private sealed class RequirementDto
    {
        public int ProjectIndex { get; set; }

        public string Description { get; set; } = string.Empty;

        public double QuantityNeeded { get; set; }

        public string Unit { get; set; } = string.Empty;

        public List<PledgeDto> Pledges { get; set; } = [];
    }

    private sealed class PledgeDto
    {
        public int ParticipantIndex { get; set; }

        public double Amount { get; set; }
    }

    private sealed class InquiryDto
    {
        public int ProjectIndex { get; set; }

        public int RaisedByParticipantIndex { get; set; }

        public int SocietyIndex { get; set; }

        public double LapseWindowHours { get; set; }

        public string Body { get; set; } = string.Empty;

        public string? Response { get; set; }
    }
}
