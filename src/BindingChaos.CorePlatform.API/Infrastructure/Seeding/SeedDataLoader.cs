using System.Text.Json;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CorePlatform.API.Infrastructure.Seeding;

/// <summary>
/// Loads shared seed data embedded in the CorePlatform.API assembly.
/// </summary>
internal static class SeedDataLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Loads the shared participant list from the embedded seed-participants.json resource.
    /// </summary>
    /// <returns>An ordered array of participant IDs matching the seed JSON.</returns>
    internal static ParticipantId[] LoadParticipants()
    {
        return LoadRecords().Select(r => ParticipantId.Create(r.Id)).ToArray();
    }

    /// <summary>
    /// Loads participant profiles (user ID + pseudonym) from the embedded seed-participants.json resource.
    /// </summary>
    /// <returns>An ordered array of seed participant profiles.</returns>
    internal static SeedParticipantProfile[] LoadParticipantProfiles()
    {
        return LoadRecords()
            .Select(r => new SeedParticipantProfile { UserId = r.Id, Pseudonym = r.Pseudonym })
            .ToArray();
    }

    private static ParticipantRecord[] LoadRecords()
    {
        using var stream = typeof(SeedDataLoader).Assembly
            .GetManifestResourceStream("BindingChaos.CorePlatform.API.Infrastructure.Seeding.seed-participants.json")
            ?? throw new InvalidOperationException("seed-participants.json embedded resource not found.");

        return JsonSerializer.Deserialize<ParticipantRecord[]>(stream, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize seed-participants.json.");
    }

    private sealed class ParticipantRecord
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Pseudonym { get; set; } = string.Empty;
    }
}

/// <summary>
/// A seed participant's stable user ID and pseudonym.
/// </summary>
internal sealed record SeedParticipantProfile
{
    /// <summary>Gets the stable internal user ID.</summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>Gets the unique pseudonym assigned to this participant.</summary>
    public string Pseudonym { get; init; } = string.Empty;
}
