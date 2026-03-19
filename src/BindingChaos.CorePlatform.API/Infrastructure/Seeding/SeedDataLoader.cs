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
        using var stream = typeof(SeedDataLoader).Assembly
            .GetManifestResourceStream("BindingChaos.CorePlatform.API.Infrastructure.Seeding.seed-participants.json")
            ?? throw new InvalidOperationException("seed-participants.json embedded resource not found.");

        var records = JsonSerializer.Deserialize<ParticipantRecord[]>(stream, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize seed-participants.json.");

        return records.Select(r => ParticipantId.Create(r.Id)).ToArray();
    }

    private sealed class ParticipantRecord
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
