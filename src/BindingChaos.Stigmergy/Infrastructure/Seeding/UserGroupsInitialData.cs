using System.Text.Json;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
using Marten.Schema;

namespace BindingChaos.Stigmergy.Infrastructure.Seeding;

/// <summary>
/// Development-only initial data seeding for the UserGroups subdomain.
/// Seeds UserGroup aggregates and activates the Commons they govern.
/// Resolves Commons by querying CommonsCreated events ordered by creation time.
/// Seed content is loaded from the embedded usergroups-seed-data.json resource.
/// </summary>
public sealed class UserGroupsInitialData : IInitialData
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ParticipantId[] _participants;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserGroupsInitialData"/> class.
    /// </summary>
    /// <param name="participants">The ordered participant array shared across all seeders.</param>
    public UserGroupsInitialData(ParticipantId[] participants)
    {
        _participants = participants;
    }

    /// <summary>
    /// Populates initial development data for UserGroups.
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
            .Where(e => e.DotNetTypeName.Contains("UserGroupFormed"))
            .AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var commonsIds = (await session.Events.QueryAllRawEvents()
            .Where(e => e.DotNetTypeName.Contains("CommonsCreated") && e.StreamKey != null)
            .OrderBy(e => e.Timestamp)
            .Select(e => e.StreamKey!)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false))
            .Distinct()
            .ToList();

        if (commonsIds.Count == 0)
        {
            return;
        }

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        var baseTime = DateTimeOffset.UtcNow.AddDays(-40);
        var timeProvider = new ControllableTimeProvider(baseTime);
        var originalTimeProvider = TimeProviderContext.Current;
        TimeProviderContext.SetCurrent(timeProvider);

        try
        {
            foreach (var dto in data.UserGroups)
            {
                if (dto.CommonsIndex >= commonsIds.Count)
                {
                    continue;
                }

                timeProvider.AdvanceHours(6);

                var commonsIdValue = commonsIds[dto.CommonsIndex];
                var commonsId = CommonsId.Create(commonsIdValue);
                var founder = _participants[dto.FounderIndex];

                // Activate the commons directly. This mirrors UserGroupFormedHandler which cannot
                // fire in the seeder context because Wolverine is bypassed.
                session.Events.Append(commonsIdValue, new CommonsActivated(commonsIdValue));

                var charter = new Charter(
                    new ContentionRules(
                        (decimal)dto.Charter.ContentionRules.RejectionThreshold,
                        TimeSpan.FromHours(dto.Charter.ContentionRules.ResolutionWindowHours)),
                    new MembershipRules(
                        Enumeration<JoinPolicy>.FromValue(dto.Charter.MembershipRules.JoinPolicy),
                        dto.Charter.MembershipRules.MemberListPublic,
                        maxMembers: null,
                        entryRequirements: null),
                    new ShunningRules((decimal)dto.Charter.ShunningRules.ApprovalThreshold));

                var userGroup = UserGroup.Form(founder, commonsId, dto.Name, dto.Philosophy, charter);

                foreach (var memberIndex in dto.MemberIndices)
                {
                    if (memberIndex < _participants.Length && memberIndex != dto.FounderIndex)
                    {
                        timeProvider.AdvanceHours(1);
                        userGroup.ApplyToJoin(_participants[memberIndex]);
                    }
                }

                session.Events.Append(userGroup.Id.Value, [.. userGroup.UncommittedEvents]);
            }

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TimeProviderContext.SetCurrent(originalTimeProvider);
        }
    }

    private static async Task<UserGroupsSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(UserGroupsInitialData).Assembly
            .GetManifestResourceStream("BindingChaos.Stigmergy.Infrastructure.Seeding.usergroups-seed-data.json")
            ?? throw new InvalidOperationException("usergroups-seed-data.json embedded resource not found in Stigmergy assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<UserGroupsSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize usergroups-seed-data.json.");
        }
    }

    private sealed class UserGroupsSeedData
    {
        public List<UserGroupDto> UserGroups { get; set; } = [];
    }

    private sealed class UserGroupDto
    {
        public int CommonsIndex { get; set; }

        public int FounderIndex { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Philosophy { get; set; } = string.Empty;

        public CharterDto Charter { get; set; } = new();

        public int[] MemberIndices { get; set; } = [];
    }

    private sealed class CharterDto
    {
        public ContentionRulesDto ContentionRules { get; set; } = new();

        public MembershipRulesDto MembershipRules { get; set; } = new();

        public ShunningRulesDto ShunningRules { get; set; } = new();
    }

    private sealed class ContentionRulesDto
    {
        public double RejectionThreshold { get; set; }

        public double ResolutionWindowHours { get; set; }
    }

    private sealed class MembershipRulesDto
    {
        /// <summary>Gets or sets the JoinPolicy numeric value: 1 = Open, 2 = InviteOnly, 3 = Approval.</summary>
        public int JoinPolicy { get; set; }

        public bool MemberListPublic { get; set; }
    }

    private sealed class ShunningRulesDto
    {
        public double ApprovalThreshold { get; set; }
    }
}
